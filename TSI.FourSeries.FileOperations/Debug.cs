
namespace TSI.FourSeries.FileOperations
{
    public class Debug
    {
        private bool _debugEnable = false;

        public ushort UshortDebugEnable
        {
            set
            {
                _debugEnable = value.Equals(1);
            }
        }

        public bool BoolDebugEnable
        {
            get
            {
                return _debugEnable;
            }
        }
    }
}
