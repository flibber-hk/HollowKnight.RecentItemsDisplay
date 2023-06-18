using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RecentItemsDisplay
{
    internal static class CoroutineHelper
    {
        public static void InvokeAfterDelay(Action a, float delay)
        {
            GameManager.instance.StartCoroutine(InvocationEnumerator(a, delay));
        }

        private static IEnumerator InvocationEnumerator(Action a, float delay)
        {
            yield return new WaitForSeconds(delay);
            a?.Invoke();
        }
    }
}
