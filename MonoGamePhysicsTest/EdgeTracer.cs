using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class EdgeTracer
    {
        private readonly Map _map;
        private readonly TileSlot _startingSlot; 
        public TileSlot FirstEdgeSlot { get; private set; }
        public TileSlot PreviousSlot;
        private MooreNeighborRelation _previousDirection;
        private readonly ILogger _logger;
        public TracerState State { get; private set; }
        private readonly Texture2D _texture;
        public EdgeTracer(Map map, TileSlot slot, ILogger logger, Texture2D texture)
        {
            _map = map;
            _startingSlot = slot;
            _previousDirection = MooreNeighborRelation.Down;
            _logger = logger;
            _texture = texture;
            State = TracerState.ReadyToStart;
        }

        public enum TracerState
        {
            ReadyToStart,
            Started,
            Finished
        }

        private readonly List<TileSlot> _perimeterSlots = new List<TileSlot>();

        public void Start()
        {
            if (State != TracerState.ReadyToStart)
            {
                return;
            }
            FirstEdgeSlot = PreviousSlot = findEdgeSlotInDirection(_map, _startingSlot, _previousDirection);
            _perimeterSlots.Add(PreviousSlot);
            State = TracerState.Started;
        }

        public void Update()
        {
            for (var i = 0; i < 10; i++)
            {
                update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var slot in _perimeterSlots)
            {
                spriteBatch.Draw(_texture, slot.ToPosition());
            }
        }

        private void update()
        {
            if (State != TracerState.Started)
            {
                return;
            }
            var maybeNextEdgeSlot = tryFindNextEdgeSlotFrom(_map, PreviousSlot, MooreNeighborRelation.GetAllCardinalsAfter(_previousDirection.Opposite()));
            if (maybeNextEdgeSlot.HasValue)
            {
                var nextEdgeSlot = maybeNextEdgeSlot.Value;
                _logger.Log("Found edge slot by going " + nextEdgeSlot.DirectionUsed.Name);
                if (nextEdgeSlot.Slot == FirstEdgeSlot)
                {
                    finished();
                    return;
                }
                _perimeterSlots.Add(nextEdgeSlot.Slot);
                PreviousSlot = nextEdgeSlot.Slot;
                _previousDirection = nextEdgeSlot.DirectionUsed;
            }
            else
            {
                finished();
            }
        }

        private void finished()
        {
            State = TracerState.Finished;
            foreach (var slot in _perimeterSlots)
            {
                _map.SetAt(slot, true);
            }
        }

        private static TileSlot findEdgeSlotInDirection(Map map, TileSlot slot, MooreNeighborRelation direction)
        {
            while (true)
            {
                var maybeNextSlot = direction.TryGetNeighborFor(slot);
                if (!maybeNextSlot.HasValue)
                {
                    return slot;
                }
                var nextSlot = maybeNextSlot.Value;
                if (map.HasTileIn(nextSlot))
                {
                    return slot;
                }
                slot = nextSlot;
            }
        }

        private static FoundEdgeSlot? tryFindNextEdgeSlotFrom(Map map, TileSlot slot, IEnumerable<MooreNeighborRelation> directions)
        {
            var found = directions
                .Select(dir => new {maybeNeighbor = dir.TryGetNeighborFor(slot), direction = dir})
                .Where(pair =>pair.maybeNeighbor.HasValue)
                .FirstOrDefault(pair =>
                {
                    var neighbor = pair.maybeNeighbor.Value;
                    return !map.HasTileIn(neighbor) && hasTileInMooreNeighborHood(map, neighbor, pair.direction);
                });
            return found == null
                ? (FoundEdgeSlot?) null
                : new FoundEdgeSlot(found.maybeNeighbor.Value, found.direction);
        }

        private static bool hasTileInMooreNeighborHood(Map map, TileSlot slot, MooreNeighborRelation startingDirection)
        {
            return MooreNeighborRelation.ALL
                .Select(r => r.TryGetNeighborFor(slot))
                .Where(n => n.HasValue)
                .Select(n => n.Value)
                .Any(map.HasTileIn);
        }

        public struct FoundEdgeSlot
        {
            public readonly TileSlot Slot;
            public readonly MooreNeighborRelation DirectionUsed;

            public FoundEdgeSlot(TileSlot slot, MooreNeighborRelation directionUsed)
            {
                Slot = slot;
                DirectionUsed = directionUsed;
            }
        }
    }
}