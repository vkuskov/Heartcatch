using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heartcatch.Core.Services
{
    public abstract class BaseSceneLoaderService : ISceneLoaderService
    {
        private const float FinishedLoadingProgress = 0.9f;

        private readonly List<AsyncOperation> operations = new List<AsyncOperation>();

        public void LoadScenes(params string[] paths)
        {
            if (operations.Count > 0)
                throw new LoadingException("Can't load new levels when previous are still loading");
            for (var i = 0; i < paths.Length; ++i)
            {
                var operation = LoadScene(paths[i], i > 0);
                operations.Add(operation);
            }
        }

        public void Update()
        {
            if (operations.Count == 0)
                return;
            if (IsAllDone())
                operations.Clear();
        }

        private bool IsLoadingFinished()
        {
            var result = true;
            foreach (var operation in operations)
                result &= operation.progress >= FinishedLoadingProgress;
            return result;
        }

        private bool IsAllDone()
        {
            var result = true;
            foreach (var operation in operations)
                result &= operation.isDone;
            return result;
        }

        protected abstract AsyncOperation LoadScene(string path, bool additive);
    }
}