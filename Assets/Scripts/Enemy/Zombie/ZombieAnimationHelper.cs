using System.Collections.Generic;

namespace ViralVial.Enemy.Zombie
{
    public static class ZombieAnimationHelper
    {
        public static Dictionary<EnemyType, Dictionary<EnemyState, int>> AnimationIndexDictionary { get; } = new Dictionary<EnemyType, Dictionary<EnemyState, int>> {
            { EnemyType.Brute,
                new Dictionary<EnemyState, int> {
                    {EnemyState.Attack, (int) BruteEnemyState.Attack},
                    {EnemyState.Death, (int) BruteEnemyState.Death},
                    { EnemyState.Idle, (int)BruteEnemyState.Idle},
                    { EnemyState.Running, (int)BruteEnemyState.Running},
                    { EnemyState.Scream, (int)BruteEnemyState.Scream},
                    { EnemyState.Walking, (int)BruteEnemyState.Walking},
                }
            },
            { EnemyType.Crone,
                new Dictionary<EnemyState, int> {
                    {EnemyState.Attack, (int)CroneEnemyState.Attack},
                    {EnemyState.Death, (int)CroneEnemyState.Death},
                    {EnemyState.Idle, (int)CroneEnemyState.Idle},
                    {EnemyState.Running, (int)CroneEnemyState.Running},
                    {EnemyState.Scream, (int)CroneEnemyState.Scream},
                    {EnemyState.Walking, (int)CroneEnemyState.Walking},
                }
            },
            { EnemyType.Grunt,
                new Dictionary<EnemyState, int> {
                    {EnemyState.Attack, (int)GruntEnemyState.Attack},
                    {EnemyState.Death, (int)GruntEnemyState.Death},
                    {EnemyState.Idle, (int)GruntEnemyState.Idle},
                    {EnemyState.Running, (int)GruntEnemyState.Running},
                    {EnemyState.Scream, (int)GruntEnemyState.Scream},
                    {EnemyState.Walking, (int)GruntEnemyState.Walking},
                }
            }
        };
    }


    public enum EnemyState
    {
        Attack, Idle, Walking, Scream, Death, Running
    }


    //animation states
    public enum GruntEnemyState
    {
        //idle, walk, scream, run, attack, death
        //5, 1, 2, 4, 3, 0
        Attack = 3,
        Idle = 5,
        Walking = 1,
        Scream = 2,
        Death = 0,
        Running = 4
    }
    public enum CroneEnemyState
    {
        //idle, walk, scream, run, attack, death
        //5, 4, 0, 2, 1, 3
        Attack = 1,
        Idle = 5,
        Walking = 4,
        Scream = 0,
        Death = 3,
        Running = 2
    }
    public enum BruteEnemyState
    {
        //idle, walk, scream, run, attack, death
        //1, 2, 3, 5, 0, 4
        Attack = 0,
        Idle = 1,
        Walking = 2,
        Scream = 3,
        Death = 4,
        Running = 5
    }

    public enum EnemyType
    {
        Brute, Grunt, Crone
    }
}