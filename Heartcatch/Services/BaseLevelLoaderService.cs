using System.Collections.Generic;
using UnityEngine;

namespace Heartcatch.Services
{
    public abstract class BaseLevelLoaderService : ILevelLoaderService
    {
        private const float FINISHED_LOADING_PROGRESS = 0.9f;
        private bool _firstPhase;

        private readonly List<AsyncOperation> _operations = new List<AsyncOperation>();

        public void LoadScenes(params string[] paths)
        {
            if (_operations.Count > 0)
                throw new LoadingException("Can't load new levels when previous are still loading");
            for (var i = 0; i < paths.Length; ++i)
            {
                var operation = loadScene(paths[i], i > 0);
                operation.allowSceneActivation = false;
                _operations.Add(operation);
            }
            _firstPhase = true;
        }

        public void Update()
        {
            if (_operations.Count == 0)
                return;
            if (_firstPhase && isLoadingFinished())
            {
                foreach (var operation in _operations)
                    operation.allowSceneActivation = true;
                _firstPhase = false;
            }
            if (!_firstPhase && isAllDone())
                _operations.Clear();
        }

        private bool isLoadingFinished()
        {
            var result = true;
            foreach (var operation in _operations)
                result &= operation.progress >= FINISHED_LOADING_PROGRESS;
            return result;
        }

        private bool isAllDone()
        {
            var result = true;
            foreach (var operation in _operations)
                result &= operation.isDone;
            return result;
        }

        protected abstract AsyncOperation loadScene(string path, bool additive);
    }
}