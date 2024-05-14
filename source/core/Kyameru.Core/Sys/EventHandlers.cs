using System.Threading.Tasks;

namespace Kyameru.Core.Sys
{
    /// <summary>
    /// Async event handler.
    /// </summary>
    /// <typeparam name="TEventData">Type of data to process.</typeparam>
    /// <param name="sender">Sender of event.</param>
    /// <param name="e">Data of event.</param>
    /// <returns>Returns a <see cref="Task"/> reference for the event.</returns>
    public delegate Task AsyncEventHandler<TEventData>(object sender, TEventData e);
}
