using Pineapple.Threading;
using System.Collections.Generic;
using System.Linq;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public interface IOpenFigiLimits
    {
        void CheckJobLimit<T>(string name, IEnumerable<T> items);

        IResourceGoverner ApiLimiter { get; }

        int ApiLimitPerMinute { get; }

        int JobLimit { get; }
    }

    internal class OpenFigiLimitWithApiKey : IOpenFigiLimits
    {
        public const int API_LIMIT_PER_MINUTE = 200;
        public const int JOB_LIMIT = 100;

        public OpenFigiLimitWithApiKey()
        {
            ApiLimiter = new ResourceGoverner(API_LIMIT_PER_MINUTE);
        }

        public IResourceGoverner ApiLimiter { get; }

        public int ApiLimitPerMinute => API_LIMIT_PER_MINUTE;

        public int JobLimit => JOB_LIMIT;

        public void CheckJobLimit<T>(string name, IEnumerable<T> items)
        {
            CheckIsNotNullOrWhitespace(nameof(name), name);
            CheckIsNotGreaterThan(nameof(items), items.Count(), JOB_LIMIT);
        }
    }

    internal class OpenFigiLimitWithOutApiKey : IOpenFigiLimits
    {
        public const int API_LIMIT_PER_MINUTE = 20;
        public const int JOB_LIMIT = 10;

        public OpenFigiLimitWithOutApiKey()
        {
            ApiLimiter = new ResourceGoverner(API_LIMIT_PER_MINUTE);
        }

        public IResourceGoverner ApiLimiter { get; }

        public int ApiLimitPerMinute => API_LIMIT_PER_MINUTE;

        public int JobLimit => JOB_LIMIT;

        public void CheckJobLimit<T>(string name, IEnumerable<T> items)
        {
            CheckIsNotNullOrWhitespace(nameof(name), name);
            CheckIsNotGreaterThan(nameof(items), items.Count(), JOB_LIMIT);
        }
    }
}
