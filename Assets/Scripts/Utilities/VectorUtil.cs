using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Utils
{

    public partial class Utilities
    {
        public static List<string> CSVLineToList(string _line)
        {
            //string[] array = ;
            List<string> rv = new List<string>(_line.Split(','));
            return rv;
        }

        public static List<int> CSVLineToIntList(string _line)
        {
            string[] array = _line.Split(',');
            List<int> rv = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                rv.Add(Int32.Parse(array[i]));
            }

            return rv;
        }

        //public static Dictionary<int, List<string>>(StringReader _stream)
        //{

        //}
    }
//    public class VectorHelp
//    {
//        /* Returns whether B is Clockwise or Anticlock wise of A, 1 & -1 */
//        public static int Sign(Vector2 _a, Vector2 _b)
//        {
//            if (_a.y * _b.x > _a.x * _b.y)
//            {
//                return -1;
//            }
//            return 1;
//        }
//        /* Rotate a vector by _angle rads */
//        public static Vector2 Rotate(Vector2 _a, float _angle)
//        {
//            Vector2 returnValue;
//            returnValue.x = _a.x * Mathf.Cos(_angle) - _a.y * Mathf.Sin(_angle);
//            returnValue.y = _a.x * Mathf.Sin(_angle) + _a.y * Mathf.Cos(_angle);
//
//            return returnValue;
//        }
//
//        //public static float Angle(Vector2 _a)
//        //{
//
//        //}
//    }
    public class VectorUtil : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}