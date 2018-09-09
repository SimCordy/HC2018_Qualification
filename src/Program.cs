using System;
using util;
using entities;
using System.Collections.Generic;

namespace hash_code_vs
{
    class Program
    {
        static void Main(string[] args)
        {
            //inputReader inputData = new inputReader("b_should_be_easy.in","b_should_be_easy.out");
            //inputReader inputData = new inputReader("c_no_hurry.in","c_no_hurry.out");
            //inputReader inputData = new inputReader("d_metropolis.in","d_metropolis.out");
            inputReader inputData = new inputReader("e_high_bonus.in","e_high_bonus.out");
            inputData.printHeaderInfo();
            inputData.printRidesInfo();
            
            inputData.execute();
            inputData.printSubmissionFile();
        }
    }
    
}
