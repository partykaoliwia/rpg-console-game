using RpgGame.Controller.Shared.Commands;
using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Server;

public sealed class ServerController
{
    private readonly GameModel _authoritativeModel;
    private readonly CommandRegistry _commands;
    private readonly GameStateMapper _mapper;
    private readonly object _actionLock = new();
    private int _stateSequence;

    public ServerController(GameModel authoritativeModel, CommandRegistry commands, GameStateMapper mapper)
    {
        _authoritativeModel = authoritativeModel;
        _commands = commands;
        _mapper = mapper;
    }

    public WelcomeDto ConnectPlayer(string playerName)
    {
        lock (_actionLock)
        {
            var player = _authoritativeModel.AddPlayer(playerName);
            return new WelcomeDto
            {
                PlayerId = player.Id,
                StateSequence = NextStateSequence(),
                State = _mapper.ToDto(_authoritativeModel)
            };
        }
    }

    public ServerStateDto? DisconnectPlayer(int playerId)
    {
        lock (_actionLock)
        {
            return _authoritativeModel.RemovePlayer(playerId)
                ? CreateStateWithoutActionLock()
                : null;
        }
    }

    public ServerStateDto Handle(int playerId, ClientCommandDto commandDto)
    {
        lock (_actionLock)
        {
            if (commandDto.PlayerId != playerId)
            {
                _authoritativeModel.RecordRejectedCommand(
                    playerId,
                    "the command player id does not match the connected player");
                return CreateStateWithoutActionLock();
            }

            try
            {
                var command = _commands.FromDto(commandDto);
                command.Execute(_authoritativeModel, playerId);
            }
            catch (InvalidOperationException exception)
            {
                _authoritativeModel.RecordRejectedCommand(playerId, exception.Message);
            }

            return CreateStateWithoutActionLock();
        }
    }

    public ServerStateDto RejectMalformedCommand(int playerId, string reason)
    {
        lock (_actionLock)
        {
            _authoritativeModel.RecordRejectedCommand(playerId, reason);
            return CreateStateWithoutActionLock();
        }
    }

    public ServerStateDto? Tick()
    {
        lock (_actionLock)
        {
            return _authoritativeModel.UpdateEnemies() ? CreateStateWithoutActionLock() : null;
        }
    }

    public ServerStateDto CreateState()
    {
        lock (_actionLock)
        {
            return CreateStateWithoutActionLock();
        }
    }

    private ServerStateDto CreateStateWithoutActionLock()
    {
        return new ServerStateDto
        {
            StateSequence = NextStateSequence(),
            State = _mapper.ToDto(_authoritativeModel)
        };
    }

    private int NextStateSequence()
    {
        return ++_stateSequence;
    }
}
