using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.TimeSystem
{
    /// Stack-safe helper for temporary time scale changes.
    /// Usage:
    ///   using (TimeScaleService.Push(0.35f)) { ... }  // auto-restore on Dispose
    public static class TimeScaleService
    {
        // Keep original fixedDelta so physics step scales correctly with timeScale
        static readonly float _baseFixedDelta = Time.fixedDeltaTime;

        // Simple LIFO stack of previous timeScales
        static readonly Stack<float> _stack = new Stack<float>(8);

        /// Current time scale (mirror of Time.timeScale)
        public static float Current => Time.timeScale;

        /// Push a new time scale. Returns a handle that restores on Dispose.
        public static IDisposable Push(float newScale)
        {
            newScale = Mathf.Clamp(newScale, 0f, 100f); // allow >1 if you ever want fast-forward
            _stack.Push(Time.timeScale);
            Apply(newScale);
            return new Scope();
        }

        /// Force-clear all pushes and restore to 1
        public static void Clear()
        {
            _stack.Clear();
            Apply(1f);
        }

        static void Apply(float scale)
        {
            // Prevent zero fixedDeltaTime; tiny epsilon if someone passes 0
            float clamped = Mathf.Max(0f, scale);
            Time.timeScale = clamped;
            Time.fixedDeltaTime = Mathf.Max(0.0001f, _baseFixedDelta * Mathf.Max(0.0001f, clamped));
        }

        /// Pops the most recent push and restores the previous scale.
        static void Pop()
        {
            float target = _stack.Count > 0 ? _stack.Pop() : 1f;
            Apply(target);
        }

        // Disposable scope that pops on Dispose (LIFO expected).
        sealed class Scope : IDisposable
        {
            bool _disposed;
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                Pop();
            }
        }
    }
}
