# Shimmer
  
地牢式肉鸽探险游戏：每一层是一个固定大小的网格地图，随机指定起点和终点,
部分层的终点会有 Boss 房。
玩家在每次战斗结束后都可以随机获得一些地图元素，并根据当前角色的情况将它们放置在地图上，如此往复直到到达本层终点

Dungeon-style 2D Rouge-Like adventure game: Each layer is a fixed size grid map with random starting and ending room.


There is a Boss room at the end of some floors. 

Players are given random map elements(every piece is a single room with the same size) at the end of each room's battle, and they can place them on the map based on the current condition(such as the door of the map pieces need to be connected), and repeat until the player reach the end of the level
  
已完成-地图块的初始化随机生成\地图块的拼接\人物基本行为逻辑  

Done - Initialization of map pieces randomly \ generates map pieces and add them to the current map \ player basic behavior logic

待更新-完成关卡后的地图块生成\怪物逻辑  

To be updated - Map block generation after completing the level \ Monster logic
