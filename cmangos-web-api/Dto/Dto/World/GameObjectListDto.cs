﻿namespace Data.Dto.World
{
    public class GameObjectListDto
    {
        public List<GameObjectDto> Items { get; set; } = new List<GameObjectDto>();
        public int Count { get; set; } = 0;
        public float Left { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Top { get; set; }
    }
}
