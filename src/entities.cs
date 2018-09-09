using System;
using System.Collections.Generic;

namespace entities{
    class Grid{
        public int R;
        public int C;
        public int F;
        public int N;
        public int B;
        public int T;

        public int current_T;

        public Grid(int R, int C, int F, int N, int B, int T){
            this.R = R;
            this.C = C;
            this.F = F;
            this.B = B;
            this.T = T;
            this.current_T = 0;
        }

        public override string ToString(){
            return String.Format("Row: {0} \t Column: {1} \t Fleet: {2} \t Bonus: {3} \t Steps: {4}",R,C,F,B,T);
        }
    }

    class Ride : IComparable<Ride>{
        public int a;
        public int b;
        public int x;
        public int y;
        public int s;
        public int f; 

        public int id;

        public Ride(int a, int b, int x, int y, int s, int f, int id){
            this.a = a;
            this.b = b;
            this.x = x;
            this.y = y;
            this.s = s;
            this.f = f;
            this.id = id;
        }

        public override string ToString(){
            return String.Format("{0} -- Start point: [{1},{2}] \t End point: [{3},{4}] \t Start: {5} \t Finish: {6}",id,a,b,x,y,s,f);
        }

        public int CompareTo(Ride element){
            return this.s-element.s;
        }

        public int duration(){
            return Math.Abs(a-x)+Math.Abs(b-y);
        }
    }

    class Car {
        public List<int> rides;
        public int assignedTo;
        public int id;

        //Car position coordinate
        public int a;
        public int b;

        public Car(int id){
            this.rides = new List<int>();
            this.id = id;
            this.a = 0;
            this.b = 0;
        }

        public static int operator -(Car c1, Ride r1) {
            return Math.Abs(c1.a-r1.a)+Math.Abs(c1.b-r1.b);
        }

        public void setPosition(int a, int b){
            this.a = a;
            this.b = b;
        }

        public override string ToString(){
            int NRides = rides.Count;
            String ridesListString = "";
            rides.ForEach(r => ridesListString = String.Concat(ridesListString," ",r));
            return String.Concat(NRides,ridesListString);;
        }
    }
}