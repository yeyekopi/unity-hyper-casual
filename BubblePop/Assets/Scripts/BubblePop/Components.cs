using System.Collections.Generic;
using Entitas;
using UnityEngine;

//logic
public class GridC : UniqueComponent<int[]> { }

public class Score : UniqueComponent<long> { }
public class Level : UniqueComponent<int> { } //0-indexed
public class LevelScore : UniqueComponent<long> { }


//anims
public class ShotPath : UniqueComponent<List<Vector3>> { } //world positions
public class Shooting : UniqueFlagComponent { }

public class Explosion2048 : FlagComponent { }
public class MergeEvent : PrimaryIndexComponent<int> { } 
public class AnimatingMergeEvent : UniqueComponent<int> { } //currently animating merge event (0-indexed)

public class Popping : IndexComponent<int> { } //to animate at specified merge event
public class Falling : IndexComponent<int> { } //to animate at specified merge event

public class SpawnRows : UniqueComponent<int> { } 

public class GameOver : UniqueFlagComponent { }
public class LevelComplete : UniqueFlagComponent { }


//rendering
public class ShooterBubble : UniqueComponent<int> { }

public class GridIndex : Component<int> { }
public class BubbleValue : Component<int> { }
public class OldBubbleValue : Component<int> { }

public class RowsAdded : UniqueComponent<int> { }

public class ViewScore : UniqueComponent<long> { }
public class ViewLevelScore : UniqueComponent<long> { }

