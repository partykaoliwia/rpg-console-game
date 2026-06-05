namespace RpgGame.Model.Game.Behaviors;

public static class EnemyBehaviors
{
    public static IEnemyBehavior RandomWalk()
    {
        return new LambdaEnemyBehavior(
            "random walk",
            (enemy, context) => context.PickRandomMove(enemy, includeStay: true));
    }

    public static IEnemyBehavior FollowSound()
    {
        return new LambdaEnemyBehavior(
            "follow sound",
            (enemy, context) =>
            {
                if (!enemy.HasHeardSound)
                {
                    return EnemyBehaviorDecision.None();
                }

                if (enemy.X == enemy.HeardSoundX && enemy.Y == enemy.HeardSoundY)
                {
                    enemy.ForgetSound();
                    return EnemyBehaviorDecision.None();
                }

                return context.PickMoveToward(enemy, enemy.HeardSoundX, enemy.HeardSoundY);
            });
    }

    public static IEnemyBehavior FleeSound()
    {
        return new LambdaEnemyBehavior(
            "flee sound",
            (enemy, context) =>
            {
                if (!enemy.HasHeardSound)
                {
                    return EnemyBehaviorDecision.None();
                }

                var decision = context.PickMoveAwayFromPoint(enemy, enemy.HeardSoundX, enemy.HeardSoundY);
                if (!decision.ShouldMove)
                {
                    enemy.ForgetSound();
                }

                return decision;
            });
    }

    public static IEnemyBehavior ChasePlayer()
    {
        return new LambdaEnemyBehavior(
            "chase player",
            (enemy, context) =>
            {
                var target = context.FindNearestVisiblePlayer(enemy);
                if (target == null)
                {
                    return EnemyBehaviorDecision.None();
                }

                enemy.ForgetSound();
                return EnemyBehaviorContext.AreAdjacent(enemy, target)
                    ? EnemyBehaviorDecision.Attack(target.Id)
                    : context.PickMoveToward(enemy, target.X, target.Y);
            });
    }

    public static IEnemyBehavior FleePlayers()
    {
        return new LambdaEnemyBehavior(
            "flee players",
            (enemy, context) =>
            {
                var visiblePlayers = context.FindVisiblePlayers(enemy);
                if (visiblePlayers.Count == 0)
                {
                    return EnemyBehaviorDecision.None();
                }

                enemy.ForgetSound();
                if (!context.HasAnyAllowedDirectionAwayFromPlayers(enemy, visiblePlayers))
                {
                    return EnemyBehaviorDecision.None();
                }

                var decision = context.PickMoveAwayFromPlayers(enemy, visiblePlayers);
                return decision.ShouldMove
                    ? decision
                    : context.PickRandomMove(enemy, includeStay: true);
            });
    }

    public static EnemyBehaviorProfile AggressiveProfile()
    {
        return new EnemyBehaviorProfile(RandomWalk(), FollowSound(), ChasePlayer());
    }

    public static EnemyBehaviorProfile CowardlyProfile()
    {
        return new EnemyBehaviorProfile(RandomWalk(), FleeSound(), FleePlayers());
    }

    public static EnemyBehaviorProfile NeutralProfile()
    {
        var randomWalk = RandomWalk();
        return new EnemyBehaviorProfile(randomWalk, randomWalk, randomWalk);
    }
}
