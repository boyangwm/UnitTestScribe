using Glimpse.Core.Extensibility;

namespace Glimpse.Core.Extensions
{
    /// <summary>
    /// Extension methods to simplify common tasks completed with <see cref="IAlternateMethodContext"/>.
    /// </summary>
    public static class AlternateMethodContextExtensions
    {
        /// <summary>
        /// Calls <c>Proceed</c> on the original implementation, if the current <c>RuntimePolicy</c> is not <c>Off</c>, and provides a corresponding <see cref="TimerResult"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timerResult">The timer result.</param>
        /// <returns>
        /// Returns <c>true</c> if <c>Proceed</c> was called on the original object, and sets <paramref name="timerResult" />. Returns <c>false</c> and sets <paramref name="timerResult" /> to <c>null</c> if <c>RuntimePolicy</c> is <c>Off</c>.
        /// </returns>
        public static bool TryProceedWithTimer(this IAlternateMethodContext context, out TimerResult timerResult)
        {
            if (context.RuntimePolicyStrategy() == RuntimePolicy.Off)
            {
                context.Proceed();
                timerResult = null;
                return false;
            }

            var timer = context.TimerStrategy();

            timerResult = timer.Time(context.Proceed);
            return true;
        }
    }
}