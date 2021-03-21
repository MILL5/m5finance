﻿using Pineapple.Threading;
using System.Collections.Generic;
using System.Linq;
using static Pineapple.Common.Preconditions;

namespace M5Finance
{
    public interface IOpenFigiLimits
    {
        void CheckJobLimit<T>(string name, IEnumerable<T> items);

        IResourceGoverner ApiLimiter { get; }
    }

    internal class OpenFigiLimitWithApiKey : IOpenFigiLimits
    {
        public const int API_LIMIT_PER_MINUTE = 250;
        public const int JOB_LIMIT = 100;

        public IResourceGoverner ApiLimiter => new ResourceGoverner(API_LIMIT_PER_MINUTE);

        public void CheckJobLimit<T>(string name, IEnumerable<T> items)
        {
            CheckIsNotNullOrWhitespace(nameof(name), name);
            CheckIsNotGreaterThan(nameof(items), items.Count(), JOB_LIMIT);
        }
    }

    internal class OpenFigiLimitWithOutApiKey : IOpenFigiLimits
    {
        public const int API_LIMIT_PER_MINUTE = 25;
        public const int JOB_LIMIT = 10;

        public IResourceGoverner ApiLimiter => new ResourceGoverner(API_LIMIT_PER_MINUTE);

        public void CheckJobLimit<T>(string name, IEnumerable<T> items)
        {
            CheckIsNotNullOrWhitespace(nameof(name), name);
            CheckIsNotGreaterThan(nameof(items), items.Count(), JOB_LIMIT);
        }
    }
}
