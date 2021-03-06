﻿/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 22/02/2009
 * Time: 10:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace IritSimulation
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public  class Utils
	{
		
		
		public  System.Random RandomGenerator;
		
		
		
		#region init Parameters
		
		public   Utils(int seed)
		{
			RandomGenerator = new Random(seed);
			
		}
		public   Utils()
		{
			
		}
		#endregion
		#region Rand
		
		 public double RandUniform(double x)
		{
			double x1=(double)RandomGenerator.Next()/int.MaxValue*x;
			return x1;
			
		}
		 public double RandUniform(double start,double end)
		{
			double x=RandUniform(end-start);
			x=x+start;
			
			return x;
			
		}
		
		 public double RandNormal(double sigma)
		{
			//Generate normal distubution random value around 0
			double ro= RandUniform(1);
			double teta= RandUniform(2*Math.PI);
			double r=Math.Sqrt(-2*Math.Pow(sigma,2)*Math.Log(1-ro));
			return r*Math.Sin(teta);
			
		}
		
		 public double RandNormal(double mean ,double sigma)
		{
			//Generate normal distubution random value around mean
			double ro= RandUniform(1);
			double teta= RandUniform(2*Math.PI);
			double r=Math.Sqrt(-2*Math.Pow(sigma,2)*Math.Log(1-ro));
			return r*Math.Sin(teta)+mean;
			
		}
		 public double RandBiNormal(double mean1 ,double sigma1,double mean2 ,double sigma2,double ratio12)
		{
			double randRation = RandUniform(1);
			double RandBiNormal;
			if (randRation>ratio12)
			{
				RandBiNormal= RandNormal(mean1,sigma1);
			}
			else
			{
				RandBiNormal= RandNormal(mean2,sigma2);
			}
			return RandBiNormal;
			
		}
		
		
		 public double RandLogNormal(LognormalParameters LP)
		{
			return RandLogNormal(LP.mu,LP.sigma);
		}
		
		 public double RandLogNormal(double tau,double sigma)
		{
			//Generate LogNormal distubution random value around tau
			double x0= RandNormal(tau,sigma);
			double x1= Math.Exp(x0);
			
			return x1;
			
		}
		
		 public LognormalParameters  CommuteLognormalParameters(double mean, double variance)
		{
			LognormalParameters LP  = new Utils.LognormalParameters();
			LP.mu = Math.Log(Math.Pow(mean,2)/Math.Sqrt(variance+Math.Pow(mean,2)));
			LP.sigma = Math.Sqrt(Math.Log(variance/Math.Pow(mean,2) + 1)) ;
			return LP;
		}
		
		public struct LognormalParameters
		{
			 public double mu;
			 public double sigma;
			 public LognormalParameters(double _mu,double _sigma)
			 {
			 mu = _mu;
			 sigma = _sigma;
			 }

		}
		
		
		 public double RandLogistic(double tau,double N0 ,double Nmax,double tFinal)
		{
			double x = RandUniform(1);
			double t= uniform2Logistic( x, tau, N0 , Nmax, tFinal);
			return t;
		}
		 public double RandExponantial(double Nf,double N0)
		{
			double x = RandUniform(1);
			double gen= uniform2Exponantial( x,Nf,N0);
			return gen;
		}
		 public double RandDecayExponantial(double mu)
		{
			//mu is positive
			double x = RandUniform(1);
			double t= -Math.Log(1-x)/mu;
			return t;
		}
		
		public  double RandBinomial(double n,double pp )
		{

			double pc =double.NaN  ;
			double plog=double.NaN ;
			double pclog=double.NaN ;
			double en=double.NaN ;
			double oldg=double.NaN ;
			
			
			double pold =(-1.0);
			Int64 nold =(-1);
			int j;
			double am;
			double em;
			double g;
			double angle;
			double p;
			double bnl;
			double sq;
			double t;
			double y;

			p =(pp <= 0.5 ? pp : 1.0-pp);
			am =n *p;
			if (n < 25)
			{
				bnl =0.0;
				for (j =1;j<=n;j++)
					if (RandUniform(1) < p)
					++bnl;
			}
			else if (am < 1.0)
			{
				g =Math.Exp(-am);
				t =1.0;
				for (j =0;j<=n;j++)
				{
					t *= RandUniform(1);
					if (t < g)
						break;
				}
				bnl =(j <= n  ? j : n);
			}
			else
			{
				if (n != nold)
				{
					en =n;
					oldg =gammln(en+1.0);
					nold =Convert.ToInt64(n);
				}
				if (p != pold)
				{
					pc =1.0-p;
					plog =Math.Log(p);
					pclog =Math.Log(pc);
					pold =p;
				}
				sq =Math.Sqrt(2.0 *am *pc);
				do
				{
					do
					{
						angle =Math.PI *RandUniform(1);
						y =Math.Tan(angle);
						em =sq *y+am;
					} while (em < 0.0 || em >= (en+1.0));
					em =Math.Floor(em);
					t =1.2 *sq*(1.0+y *y)*Math.Exp(oldg-gammln(em+1.0) -gammln(en-em+1.0)+em *plog+(en-em)*pclog);
				} while (RandUniform(1) > t);
				bnl =em;
			}
			if (p != pp)
				bnl =n-bnl;
			return bnl;
		}
		
		
		#endregion
		#region Convertion functions
		
		 public double uniform2Logistic(double x,double tau,double N0 ,double Nmax,double tFinal)
		{
			double t = tau*Math.Log((Nmax-Nmax*x+Math.Exp(tFinal/tau)*Nmax*x-N0+Math.Exp(tFinal/tau)*N0+x*N0-Math.Exp(tFinal/tau)*x*N0)/(Nmax-N0+Math.Exp(tFinal/tau)*N0+x*N0-Math.Exp(tFinal/tau)*x*N0));
			return t;
		}
		
		 public double uniform2Exponantial(double x,double Nf,double N0)
		{
			double Fgen = Math.Log(Nf/N0,2);
			double gen = (Math.Log((1-Math.Pow(2,Fgen))*(1/(1-Math.Pow(2,Fgen))-x)))/(Math.Log(2));
			
			return gen;
		}
		
		
		 public double NLogistic(double t,double tau,double N0 ,double Nmax)
		{
			double N = (Math.Exp(t/tau) * Nmax * N0)/(Nmax - N0 + N0*Math.Exp(t/tau));
			return N;
		}
		
	
		 public double NExponantial(double gen,double N0)
		{
			double N = Math.Pow(2,gen)*N0;
			N=Math.Round(N);
			return N;
		}
		#endregion
		

		
		#region Binomial utils
		
		 public double gammln(double xx) {

			double [] cof={76.18009173,-86.50532033,24.01409822,
				-1.231739516,0.120858003e-2,-0.536382e-5};
			int j;

			double x = xx - 1.0;
			double tmp = x + 5.5;
			tmp -= (x+0.5)*Math.Log(tmp);
			double ser=1.0;
			for (j=0;j<=5;j++) {
				x += 1.0;
				ser += cof[j]/x;
			}
			return -tmp+Math.Log(2.50662827465*ser);
		}
		

		#endregion

		
		
		
	}
	
	
}
