using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Misc {
	
public class Point {
	public int x;
	public int y;
	
	public Point(int x,int y){
		this.x = x;
		this.y = y;
	}
	public string toString(){ return x + " " + y + "|";}
}

public class Pair<T1,T2> {
    public T1 a;
    public T2 b;
	
	public Pair(){}
    public Pair(T1 a,T2 b) {
        this.a = a;
        this.b = b;
    }
}

public class Geometry {

	public static bool is_n_within_range(int n,int a,int b){
		return ((a > b) ? (n >= b && n <= a) : (n >= a && n <= b));
	}

	public static bool is_n_within_range_float(float n,float a,float b){
		return ((a > b) ? (n >= b && n <= a) : (n >= a && n <= b));
	}
	
	/** Returns whether the line ((x1,y1),(x2,y2)) intersects line ((x3,y3),(x4,y4)). **/
	public static bool line_intersects_line(int x1,int y1,int x2,int y2,int x3,int y3,int x4,int y4){
		int xmin,xmax,ymin,ymax;
		if (x1 < x2){ xmin = x1; xmax = x2; }else{ xmin = x2; xmax = x1;}
		if (y1 < y2){ ymin = y1; ymax = y2; }else{ ymin = y2; ymax = y1;}
		int xmin2,xmax2,ymin2,ymax2;
		if (x3 < x4){ xmin2 = x3; xmax2 = x4; }else{ xmin2 = x4; xmax2 = x3;}
		if (y3 < y4){ ymin2 = y3; ymax2 = y4; }else{ ymin2 = y4; ymax2 = y3;}

		if (!(xmax+1 <= xmin2 || xmax2+1 <= xmin || ymax+1 <= ymin2 || ymax2+1 <= ymin)){
			// line = ax+b
			// line 1 flat :
			if (y1 == y2){
				// line 2 flat :
				if (y3 == y4){ return ((y1 == y3) && (is_n_within_range(x1,x3,x4) || is_n_within_range(x2,x3,x4) || is_n_within_range(x3,x1,x2) || is_n_within_range(x4,x1,x2)));}
				// line 2 wall :
				else if (x3 == x4){ return (is_n_within_range(y1,y3,y4) && is_n_within_range(x3,x1,x2)); }
				// line 2 ???? :
				else{
					int centerX = (x1+x2+x3+x4)/4; int centerY = (y1+y2+y3+y4)/4;
					x1 -= centerX; x2 -= centerX; x3 -= centerX; x4 -= centerX;
					y1 -= centerY; y2 -= centerY; y3 -= centerY; y4 -= centerY;

					float a2 = (float)(y3-y4)/(float)(x3-x4);
					float b2 = y3 - a2*x3;
					float x = (b2-y1) / (-a2);
					return (is_n_within_range_float(x,x1,x2) && is_n_within_range_float(x,x3,x4));
				}
			// line 1 wall :
			}else if (x1 == x2){
				// line 2 flat :
				if (y3 == y4){ return (is_n_within_range(x1,x3,x4) && is_n_within_range(y3,y1,y2)); }
				// line 2 wall :
				else if (x3 == x4){ return ((x1 == x3) && (is_n_within_range(y1,y3,y4) || is_n_within_range(y2,y3,y4) || is_n_within_range(y3,y1,y2) || is_n_within_range(y4,y1,y2)));}
				// line 2 ???? :
				else{
					int centerX = (x1+x2+x3+x4)/4; int centerY = (y1+y2+y3+y4)/4;
					x1 -= centerX; x2 -= centerX; x3 -= centerX; x4 -= centerX;
					y1 -= centerY; y2 -= centerY; y3 -= centerY; y4 -= centerY;

					float a2 = (float)(y3-y4)/(float)(x3-x4);
					float b2 = y3 - a2*x3;
					float y = a2 * x1 + b2;
					return (is_n_within_range_float(y,y1,y2) && is_n_within_range_float(y,y3,y4));
				}
			// line 1 ???? :
			}else{
				// line 2 flat :
				if (y3 == y4){
					int centerX = (x1+x2+x3+x4)/4; int centerY = (y1+y2+y3+y4)/4;
					x1 -= centerX; x2 -= centerX; x3 -= centerX; x4 -= centerX;
					y1 -= centerY; y2 -= centerY; y3 -= centerY; y4 -= centerY;

					float a1 = (float)(y1-y2)/(float)(x1-x2);
					float b1 = y1 - a1*x1;
					float x = (y3-b1) / (a1);
					return (is_n_within_range_float(x,x1,x2) && is_n_within_range_float(x,x3,x4));
				}
				// line 2 wall :
				else if (x3 == x4){
					int centerX = (x1+x2+x3+x4)/4; int centerY = (y1+y2+y3+y4)/4;
					x1 -= centerX; x2 -= centerX; x3 -= centerX; x4 -= centerX;
					y1 -= centerY; y2 -= centerY; y3 -= centerY; y4 -= centerY;

					float a1 = (float)(y1-y2)/(float)(x1-x2);
					float b1 = y1 - a1*x1;
					float y = a1 * x3 + b1;
					return (is_n_within_range_float(y,y1,y2) && is_n_within_range_float(y,y3,y4));
				}
				// line 2 ???? :
				else{
					int centerX = (x1+x2+x3+x4)/4; int centerY = (y1+y2+y3+y4)/4;
					x1 -= centerX; x2 -= centerX; x3 -= centerX; x4 -= centerX;
					y1 -= centerY; y2 -= centerY; y3 -= centerY; y4 -= centerY;

					float a1 = (float)(y1-y2)/(float)(x1-x2);
					float b1 = y1 - a1*x1;
					float a2 = (float)(y3-y4)/(float)(x3-x4);
					float b2 = y3 - a2*x3;
					float x = (b2-b1) / (a1-a2);
					return (is_n_within_range_float(x,x1,x2) && is_n_within_range_float(x,x3,x4));
				}
			}
		}else{
			return false;
		}
	}
}

public class MiscBinary {

	public static int countBits(byte n){
		return (n&1) + ((n>>1)&1) + ((n>>2)&1) + ((n>>3)&1) + ((n>>4)&1) + ((n>>5)&1) + ((n>>6)&1) + ((n>>7)&1);
	}

	public static int nbDifferentBits(byte n1,byte n2){
		return countBits((byte)(n1^n2));
	}

}

}