using Prism.Mvvm;

namespace NodeManager.Models
{
    internal class GlobalProperty : BindableBase
    {
        private static GlobalProperty _globalProperty;

        public GlobalProperty()
        {
        }

        public static GlobalProperty GeInstance()
        {
            if (_globalProperty == null)
            {
                _globalProperty = new GlobalProperty();
            }

            return _globalProperty;
        }
    }
}