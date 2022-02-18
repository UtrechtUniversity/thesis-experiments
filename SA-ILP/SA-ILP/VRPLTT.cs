﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA_ILP
{
    internal static class VRPLTT
    {
        private static double CalculateSpeed(double heightDiff, double length, double vehicleMass,double powerInput)
        {
            double speed = 25;
            double slope = Math.Atan(heightDiff / length) * Math.PI / 180;
            double requiredPow = CalcRequiredForce(speed / 3.6, vehicleMass, slope);
            double orignalPow = requiredPow;
            if (powerInput >= requiredPow)
            {
                    return speed;
            }
            while (speed > 0)
            {
                if (powerInput >= requiredPow)
                {
                    if (orignalPow + requiredPow - 2 * powerInput < 0)
                        speed += 0.01;
                    return speed;
                }

                speed -= 0.01;
                requiredPow = CalcRequiredForce(speed / 3.6, vehicleMass, slope);
            }
            return 0;


        }

        public static double CalculateTravelTime(double heightDiff, double length, double vehicleMass, double powerInput)
        {
            if (length == 0)
                return 0;
            //Speed in m/s
            double speed = CalculateSpeed(heightDiff, length, vehicleMass, powerInput)/3.6;

            //Return travel time in minutes
            return length * 1000  / speed /60;
        }

        public static (double[,] distances,List<Customer> customers) ParseVRPLTTInstance(string file)
        {
            List<Customer> customers = new List<Customer>();
            
            using(StreamReader sr = new StreamReader(file))
            {
                var len = sr.ReadLine().Split(',').Length - 8;

                double[,] distances = new double[len, len];
                string line = "";
                while((line = sr.ReadLine()) != null)
                {
                    var lineSplit = line.Split(',');
                    int id = int.Parse(lineSplit[0]);
                    double x = double.Parse(lineSplit[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                    double y = double.Parse(lineSplit[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                    double elevation = double.Parse(lineSplit[3], NumberStyles.Any, CultureInfo.InvariantCulture);
                    double demand, twstart, twend,serviceTime;
                    if (lineSplit[4] != "")
                        demand = double.Parse(lineSplit[4], NumberStyles.Any, CultureInfo.InvariantCulture);
                    else
                        demand = 0;
                    //double twstart = double.Parse(lineSplit[5]);
                    //double twend = double.Parse(lineSplit[6]);

                    if (lineSplit[5] != "")
                        twstart = double.Parse(lineSplit[5], NumberStyles.Any, CultureInfo.InvariantCulture);
                    else
                        twstart = 0;
                    if (lineSplit[6] != "")
                        twend = double.Parse(lineSplit[6], NumberStyles.Any, CultureInfo.InvariantCulture);
                    else
                        twend = 0;
                    if (lineSplit[7] != "")
                        serviceTime = double.Parse(lineSplit[7], NumberStyles.Any, CultureInfo.InvariantCulture);
                    else
                        serviceTime = 0;

                    //double serviceTime = double.Parse(lineSplit[7]);
                    var customer = new Customer(id,x,y,demand,twstart,twend,serviceTime,elevation);
                    for(int i = 8; i< lineSplit.Length; i++)
                    {
                        distances[id, i - 8] = double.Parse(lineSplit[i], NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                    customers.Add(customer);
                }
                return (distances,customers);
            }

        }


        public static double CalcRequiredForce(double v, double mass, double slope)
        {
            double Cd = 1.18;
            double A = 0.83;
            double Ro = 1.18;
            double Cr = 0.01;
            double g = 9.81;

            return ((Cd * A * Ro * Math.Pow(v, 2) / 2) + Cr * mass * g * Math.Cos(Math.Atan(slope)) + mass * g * Math.Sin(Math.Atan(slope))) * v / 0.95;
        }
    }
}