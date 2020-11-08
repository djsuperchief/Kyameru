using System;
namespace Kyameru.Core.BaseComponents
{
    /// <summary>
    /// Kyameru works on chain of responsibility pattern. All components implement SetNext to ensure handing over of control to the next component in line.
    /// </summary>
    internal abstract class COR
    {
        public COR()
        {
        }
    }
}
