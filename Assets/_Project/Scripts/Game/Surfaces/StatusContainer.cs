using FB.Game.Blame;

namespace FB.Game.Surfaces
{
    /// <summary>Status flags of one surface, each remembering the <see cref="Cause"/> that applied it.</summary>
    public sealed class StatusContainer
    {
        private SurfaceStatus _flags;
        private Cause _wetCause;
        private Cause _burnCause;
        private Cause _ruinCause;
        private Cause _electrifiedCause;

        public SurfaceStatus Flags => _flags;

        public bool Has(SurfaceStatus status) => (_flags & status) != 0;

        /// <summary>Returns true if the status was not set before.</summary>
        public bool Add(SurfaceStatus status, Cause cause)
        {
            bool added = !Has(status);
            _flags |= status;
            SetCause(status, cause);
            return added;
        }

        /// <summary>Returns true if the status was set before.</summary>
        public bool Remove(SurfaceStatus status)
        {
            bool removed = Has(status);
            _flags &= ~status;
            return removed;
        }

        public Cause CauseOf(SurfaceStatus status)
        {
            switch (status)
            {
                case SurfaceStatus.Wet: return _wetCause;
                case SurfaceStatus.Burning: return _burnCause;
                case SurfaceStatus.Ruined: return _ruinCause;
                case SurfaceStatus.Electrified: return _electrifiedCause;
                default: return null;
            }
        }

        private void SetCause(SurfaceStatus status, Cause cause)
        {
            switch (status)
            {
                case SurfaceStatus.Wet: _wetCause = cause; break;
                case SurfaceStatus.Burning: _burnCause = cause; break;
                case SurfaceStatus.Ruined: _ruinCause = cause; break;
                case SurfaceStatus.Electrified: _electrifiedCause = cause; break;
            }
        }
    }
}
