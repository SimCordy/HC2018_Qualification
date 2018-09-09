using System;
using entities;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace util{
    class inputReader{
        String path;
        String submissionPath;
        int N;
        bool header;

        public List<Ride> rides;

        public List<Ride> discardedRides;

        public Grid grid;

        Dictionary<int,Car> cars = new Dictionary<int, Car>();

        public inputReader(string path, string submissionPath){
            this.path = path;
            this.submissionPath = submissionPath;
            this.header = true;
            this.rides = new List<Ride>();
            this.discardedRides = new List<Ride>();

            readInputFile();
            initializeCar();
        }

        public void readInputFile(){
           try {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(this.path))
                {
                    int id = 0;
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadLine();
                    while (line != null){
                        if(header){
                            Console.WriteLine("Reading Header....");
                            string[] headerValues = line.Split();
                            N = int.Parse(headerValues[3]);
                            grid = new Grid(int.Parse(headerValues[0]), int.Parse(headerValues[1]), int.Parse(headerValues[2]), N, int.Parse(headerValues[4]), int.Parse(headerValues[5]));
                            header = false;
                            Console.WriteLine("Reading rides data....");
                        }else{
                            string[] ride = line.Split();
                            Ride r = new Ride(int.Parse(ride[0]), int.Parse(ride[1]), int.Parse(ride[2]), int.Parse(ride[3]), int.Parse(ride[4]), int.Parse(ride[5]), id);
                            rides.Add(r);
                            id++;
                        }
                        line = sr.ReadLine();
                    }
                }
                //Sorting the rides list by start step
                rides.Sort();
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void printHeaderInfo(){
            Console.WriteLine("**Header Info**");
            Console.WriteLine(grid.ToString());
            Console.WriteLine("**End**");
        }

        public void printRidesInfo(){
            Console.WriteLine("**Rides Info**");
            foreach(var r in rides){
                Console.WriteLine("{0}",r.ToString());
            }
            Console.WriteLine("**End**");
        }

        public void AssignRideToCar(List<int> freeCar){

            foreach(int id in freeCar){
                Car c;
                if(cars.ContainsKey(id)) {
                    c = cars[id];
                } else {
                    Console.WriteLine("**ERROR: Car not found!**");
                    break;
                }
                
                List<Ride> notAssignedRides = new List<Ride>(rides);
                notAssignedRides.AddRange(discardedRides);
                notAssignedRides.Sort();

                int j = 0;
                Boolean check = false;
                do{
                    Ride r = notAssignedRides[j];
                    check = checkAssignation(c,r);
                    rides.RemoveAll(ridesElement => ridesElement.id == r.id); //Remove the rides from the list

                    //If the ride has not been assinged it is added to the discardedRides list, another car could carry out it
                    if(!check && !discardedRides.Contains(r)) discardedRides.Add(r);
                    j++;

                }while(!check && j<notAssignedRides.Count);
                
                //Updating car in the dictionary
                cars[c.id] = c;
            }
        }

        public void execute(){
            int minAssingnedTo = 0; 
            while(rides.Count != 0){
                //logRidesInfo();
                if(cars.Count != 0){
                    minAssingnedTo = cars.Values.Min(carElement => carElement.assignedTo);
                } 
                grid.current_T = minAssingnedTo; //Setting the current step to the next minimum step in wich a car is available for a ride
                //Accumulating all the available car for a ride in the current step in freeCar
                List<int> freeCar = new List<int>();
                foreach( KeyValuePair<int, Car> carElement in cars ) {
                    if(carElement.Value.assignedTo == grid.current_T) freeCar.Add(carElement.Value.id);
                }
                if(freeCar.Count != 0) AssignRideToCar(freeCar);
                else Console.WriteLine("**WARN: There are no available car**");
            }  
            if(discardedRides.Count != 0) Console.WriteLine("**There are discarded Rides:{0}**",discardedRides.Count);
        }

        void logRidesInfo(){
            Console.WriteLine("Remaining rides:{0}\tDiscarded rides:{1}",rides.Count,discardedRides.Count);
            String ridesId = "";
            rides.ForEach(r =>{
                ridesId += r.id+",";
            });
            Console.WriteLine("**Remaining discarded rides ID:**");
            String discarderRidesId = "";
            discardedRides.ForEach(dr =>{
                discarderRidesId += dr.id+",";
            });

            Console.WriteLine("**Remaining rides ID:**");
            Console.WriteLine(ridesId);
            Console.WriteLine("**Remaining discarded rides ID:**");
            Console.WriteLine(discarderRidesId);

        }

        public void log(Car c, Ride r, int startDistanceDuration, int rideDuration, int totalTime, int endStepEstimation, int current_T){
            Console.WriteLine("*****************************");
            Console.WriteLine("Car:{0}\tRide:{1}",c.id,r.id);
            Console.WriteLine("Car start position:[{0},{1}]",c.a,c.b);
            Console.WriteLine("Ride start position:[{0},{1}]",r.a,r.b);
            Console.WriteLine("Start distance duration:{0}",startDistanceDuration);
            Console.WriteLine("Ride Duration:{0}",rideDuration);
            Console.WriteLine("Total time:{0}",totalTime);
            Console.WriteLine("End step estimation:{0}",endStepEstimation);
            Console.WriteLine("Ride end step:{0}",r.f);
            Console.WriteLine("Current T:{0}",current_T);
            Console.WriteLine("*****************************");
        }

        Boolean checkAssignation(Car c, Ride r){
            int startDistanceDuration = c-r;
            int rideDuration = r.duration();
            int totalTime = startDistanceDuration + rideDuration;
            int endStepEstimation = grid.current_T + totalTime;
            Boolean check = false;
            if(startDistanceDuration == 0){
                if(!c.rides.Contains(r.id)) {
                    //log(c,r,startDistanceDuration,rideDuration,totalTime,endStepEstimation,grid.current_T);
                    c.rides.Add(r.id); //Assign the ride to the car
                    c.setPosition(r.x,r.y); //At the end of the ride the car will be in the ride final position
                    c.assignedTo = endStepEstimation; //The the car will be in service up to the endStepEstimation
                    check = true;
                }
            } else {
                if(endStepEstimation <= r.f){
                    if(!c.rides.Contains(r.id)) {
                        //log(c,r,startDistanceDuration,rideDuration,totalTime,endStepEstimation,grid.current_T);
                        c.rides.Add(r.id); //Assign the ride to the car
                        c.setPosition(r.x,r.y); //At the end of the ride the car will be in the ride final position
                        c.assignedTo = endStepEstimation; //The the car will be in service up to the endStepEstimation
                        check = true;
                    }
                }
            }
            return check;
        }

        public void initializeCar(){
            for(int id=0; id<grid.F;id++){
                Car c = new Car(id);
                cars.Add(c.id,c);
            }
            Console.WriteLine("**Car fleet instantiated:{0}**",cars.Count);
        }

        public void printSubmissionFile(){
            try{
                using (StreamWriter subOut = new StreamWriter(this.submissionPath))
                {
                    Console.WriteLine("**Creating submission file**");
                    foreach(KeyValuePair<int,Car> c in cars){
                        subOut.WriteLine(c.Value.ToString());
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("The file could not be write:");
                Console.WriteLine(e.Message);
            }
        }
    }
}