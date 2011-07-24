/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 24/07/2011
 * Time: 15:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.IO;

namespace IritSimulation
{
	/// <summary>
	/// Description of TubeWrapper.
	/// </summary>
	public class TubeWrapper:Tube
	{

		private ManualResetEvent _doneEvent;
		
		public TubeWrapper(TubeParameters TP,double MaxTime, ManualResetEvent doneEvent): base(TP,MaxTime)
		{
			_doneEvent = doneEvent;
		}
		
		
		// Wrapper method for use with thread pool.
		public void ThreadPoolCallback(Object threadContext)
		{
			int threadIndex = (int)threadContext;
			//Console.WriteLine("thread {0} started...", threadIndex);
			//do work
			SimulateTube.GrowToNmax(this);
			
			
			
			int sim = (int)threadContext;
				
				
				string FName = @"ParalelSim" + sim.ToString();
				string filename = FName+  ".txt";
				System.IO.StreamWriter SR = new StreamWriter(filename, false);

				
				
				double[,] N = this.NBacteria;
				double[] t = this.Time;
				
				for (int i=0; i<N.GetLength(0); i++)
				{
					SR.Write("{0}\t",t[i]);
					for (int j=0; j<N.GetLength(1); j++)
					{
						SR.Write("{0}\t",N[i,j]);
					}
					SR.WriteLine();
					
				}
				SR.Close();
			
			
			
			
			//Console.WriteLine("thread {0} result calculated...", threadIndex);
			int count = Interlocked.Decrement(ref Program._numerOfThreadsNotYetCompleted);
			if (count == 0)
			{
				_doneEvent.Set();
			}
		}
		
		
	}
}
