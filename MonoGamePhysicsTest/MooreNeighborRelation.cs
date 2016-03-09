using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoGamePhysicsTest
{
    [DebuggerDisplay("{Name}")]
    public class MooreNeighborRelation
    {
        private readonly int _colOffset;
        private readonly int _rowOffset;
        public readonly string Name;
        private MooreNeighborRelation(string name, int colOffset, int rowOffset)
        {
            Name = name;
            _colOffset = colOffset;
            _rowOffset = rowOffset;
        }

        public IEnumerable<MooreNeighborRelation> OtherCardinalsInClockwiseOrder()
        {
            var others = CARDINALS.ToList();
            var selfIndex = others.IndexOf(this);
            var beforeSelf = others.Take(selfIndex);
            var afterSelf = others.Skip(selfIndex + 1);
            return afterSelf.Concat(beforeSelf);
        }

        private bool IsOpposite(MooreNeighborRelation other)
        {
            return _colOffset == -other._colOffset && _rowOffset == -other._rowOffset;
        }

        public TileSlot? TryGetNeighborFor(TileSlot slot)
        {
            return TileSlot.TryCreate(
                slot.ColumnIndex + _colOffset,
                slot.RowIndex + _rowOffset);
        }

        //Instances.
        public static readonly MooreNeighborRelation Up        = new MooreNeighborRelation("Up",        0, -1);
        public static readonly MooreNeighborRelation UpRight   = new MooreNeighborRelation("UpRight",   1, -1);
        public static readonly MooreNeighborRelation Right     = new MooreNeighborRelation("Right",     1,  0);
        public static readonly MooreNeighborRelation DownRight = new MooreNeighborRelation("DownRight", 1,  1);
        public static readonly MooreNeighborRelation Down      = new MooreNeighborRelation("Down",      0,  1);
        public static readonly MooreNeighborRelation DownLeft  = new MooreNeighborRelation("DownLeft", -1,  1);
        public static readonly MooreNeighborRelation Left      = new MooreNeighborRelation("Left",     -1,  0);
        public static readonly MooreNeighborRelation UpLeft    = new MooreNeighborRelation("UpLeft",   -1, -1);

        public static IEnumerable<MooreNeighborRelation> CARDINALS = new[]
        {
            Up,
            Right,
            Down,
            Left
        };

        public static IEnumerable<MooreNeighborRelation> ALL = new[]
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft
        };

        private readonly static Dictionary<MooreNeighborRelation, IEnumerable<MooreNeighborRelation>> AllCardinalsExceptOpposite =
                ALL.Select(self =>
                {
                    var others = CARDINALS.ToList();
                    var selfIndex = others.IndexOf(self);
                    var beforeSelf = others.Take(selfIndex);
                    var afterWithSelf = others.Skip(selfIndex);
                    return new
                    {
                        self,
                        cardinals = (afterWithSelf.Concat(beforeSelf).Where(r => !self.IsOpposite(r))).ToArray()
                    };
                }).ToArray()
                .ToDictionary(pair => pair.self, pair => pair.cardinals.AsEnumerable());

        public static IEnumerable<MooreNeighborRelation> GetAllCardinalsExceptOpposite(MooreNeighborRelation self)
        {
            return AllCardinalsExceptOpposite[self];
        }

        private readonly static Dictionary<MooreNeighborRelation, IEnumerable<MooreNeighborRelation>> AllExceptOpposite =
                ALL.Select(self =>
                {
                    var others = ALL.ToList();
                    var selfIndex = others.IndexOf(self);
                    var beforeSelf = others.Take(selfIndex);
                    var afterWithSelf = others.Skip(selfIndex);
                    return new
                    {
                        self,
                        cardinals = (afterWithSelf.Concat(beforeSelf).Where(r => !self.IsOpposite(r))).ToArray()
                    };
                }).ToArray()
                .ToDictionary(pair => pair.self, pair => pair.cardinals.AsEnumerable());

        public static IEnumerable<MooreNeighborRelation> GetAllExceptOpposite(MooreNeighborRelation self)
        {
            return AllExceptOpposite[self];
        }

        public MooreNeighborRelation Opposite()
        {
            return ALL.Single(mnr => mnr.IsOpposite(this));
        }

        private readonly static Dictionary<MooreNeighborRelation, IEnumerable<MooreNeighborRelation>> AllStartingAfter =
                ALL.Select(self =>
                {
                    var others = ALL.ToList();
                    var selfIndex = others.IndexOf(self);
                    var beforeSelf = others.Take(selfIndex);
                    var after = others.Skip(selfIndex + 1);
                    return new
                    {
                        self,
                        all = after.Concat(beforeSelf)
                    };
                }).ToArray()
                .ToDictionary(pair => pair.self, pair => pair.all.ToArray().AsEnumerable());

        public static IEnumerable<MooreNeighborRelation> GetAllStartingAfter(MooreNeighborRelation self)
        {
            return AllStartingAfter[self];
        }

        private readonly static Dictionary<MooreNeighborRelation, IEnumerable<MooreNeighborRelation>> AllStartingWith =
                ALL.Select(self =>
                {
                    var others = ALL.ToList();
                    var selfIndex = others.IndexOf(self);
                    var beforeSelf = others.Take(selfIndex - 1);
                    var after = others.Skip(selfIndex);
                    return new
                    {
                        self,
                        all = after.Concat(beforeSelf)
                    };
                }).ToArray()
                .ToDictionary(pair => pair.self, pair => pair.all.ToArray().AsEnumerable());

        public static IEnumerable<MooreNeighborRelation> GetAllStartingWith(MooreNeighborRelation self)
        {
            return AllStartingWith[self];
        }

        private readonly static Dictionary<MooreNeighborRelation, IEnumerable<MooreNeighborRelation>> AllCardinalsAfter =
                ALL.Select(self =>
                {
                    var others = CARDINALS.ToList();
                    var selfIndex = others.IndexOf(self);
                    var beforeSelf = others.Take(selfIndex);
                    var after = others.Skip(selfIndex + 1);
                    return new
                    {
                        self,
                        all = after.Concat(beforeSelf)
                    };
                }).ToArray()
                .ToDictionary(pair => pair.self, pair => pair.all.ToArray().AsEnumerable());

        public static IEnumerable<MooreNeighborRelation> GetAllCardinalsAfter(MooreNeighborRelation self)
        {
            return AllCardinalsAfter[self];
        }
    }
}