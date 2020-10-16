using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public static class Quicksort
{
    public static void QuicksortByDistance(ref Transform[] arrayToSort, Transform objectToCompareWith) {
        QuickSort(ref arrayToSort, 0, arrayToSort.Length -1, objectToCompareWith);
    }

     static void QuickSort(ref Transform[] array, int left, int right, Transform current) {
         if (left >= right) {
             return;
         }

         int pivot = Partiton(ref array, left, right, current);
         QuickSort(ref array, left, pivot - 1, current);
         QuickSort(ref array, pivot + 1, right, current);
     }

     static int Partiton(ref Transform[] array, int left, int right, Transform current) {
         float pivotValue = GetValue(array[right], current);
         int lesserCounter = left - 1;
         for (int greaterCounter = left; greaterCounter < right; greaterCounter++) {
             if (GetValue(array[greaterCounter], current) < pivotValue) {
                 lesserCounter++;
                 Swap(ref array[lesserCounter], ref array[greaterCounter]);
             }
         }
         Swap(ref array[lesserCounter+1], ref array[right]);
         return lesserCounter+1;
     }
    
     static void Swap<T> (ref T lhs, ref T rhs) {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

     static float GetValue(Transform target, Transform current) {
         Vector3 vectorToTarget = target.position - current.position;
         return vectorToTarget.sqrMagnitude;
     }
}
