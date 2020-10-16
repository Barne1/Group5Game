using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSortTest : MonoBehaviour
{
    [SerializeField] int[] array;
    private void Awake() {
        QuickSort(array);
    }

    public void QuickSort(int[] array) {
        QuickSort(array, 0, array.Length-1);
    }
    
    public void QuickSort(int[] array, int left, int right) {
        //base case, only one or zero items left
        if (left >= right) {
            return;
        }

        int pivot = Partition(array, left, right);
        //sort left side of pivot
        QuickSort(array, left, pivot -1);
        //sort right side of pivot
        QuickSort(array, pivot+1, right);
    }

    public int Partition(int[] array, int left, int right) {
        int pivot = array[right];
        //all indexes smaller than this should be smaller than pivot
        int lesserCounter = left - 1;
        //all indexes between greaterCounter and lesserCounter should be larger than Pivot
        for (int greaterCounter = left; greaterCounter < right; greaterCounter++) {
            //if the greaterCounter meets a "lesser" number, it sends it over to the "lesser" side
            if (array[greaterCounter] < pivot) {
                //lesserCounter goes to the first greater number and sends it to the greater side while
                //receiving the lesser number in its place
                lesserCounter++;
                Swap(ref array[lesserCounter], ref array[greaterCounter]);
            }
            //otherwise just keep going, as the greater number will be in front of lesserCounter
        }
        //put the pivot in the middle
        Swap(ref array[lesserCounter+1], ref array[right]);
        //return the pivot
        return lesserCounter + 1;
    }
    
    public void Swap<T> (ref T lhs, ref T rhs) {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

}
