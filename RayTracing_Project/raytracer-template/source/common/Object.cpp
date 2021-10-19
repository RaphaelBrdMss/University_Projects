//////////////////////////////////////////////////////////////////////////////
//
//  --- Object.cpp ---
//  Created by Brian Summa
//
//////////////////////////////////////////////////////////////////////////////


#include "common.h"

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
Object::IntersectionValues Sphere::intersect(vec4 p0, vec4 V){
    IntersectionValues result;
    //TODO: Ray-sphere setup


    result.t=this->raySphereIntersection(p0,V);

    vec3 p0_3=vec3(p0.x,p0.y,p0.z);
    vec3 V_3=vec3(V.x,V.y,V.z);
    vec3 tempo=p0_3+result.t*V_3;


    result.P=vec4(tempo.x,tempo.y,tempo.z,1.0);

    vec3 p=vec3(result.P.x,result.P.y,result.P.z);
    vec3 center3=vec3(this->center.x,this->center.y,this->center.z);
    result.N=result.P - this->center;
    result.name=name;





    return result;
}

/* -------------------------------------------------------------------------- */
/* ------ Ray = p0 + t*V  sphere at origin center and radius radius    : Find t ------- */
double Sphere::raySphereIntersection(vec4 p0, vec4 V){
    double t   = std::numeric_limits< double >::infinity();
    //TODO: Ray-sphere intersection;
    vec3 p03=vec3(p0.x,p0.y,p0.z);
    vec3 V3=vec3(V.x,V.y,V.z);
    //r(t)=o+t d
    //||p-c||²-r²=0


    double a=dot(V3,V3);
    vec3 inter=p03-this->center;
    double b=2*dot(V3,inter);
    double c=dot(inter,inter)-this->radius*this->radius;

    double disc=b*b-4*a*c;

    if(disc<0) return t;
    else if(disc==0) return (-b/2-a);
    else
    {
        double x1=(-b-sqrt(disc))/(2*a);
        double x2=(-b+sqrt(disc))/(2*a);
        if(x1<x2)
            return x1;
        else
            return x2;
    }
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */

Object::IntersectionValues Square::intersect(vec4 p0, vec4 V){
    IntersectionValues result;
    //TODO: Ray-square setup

    result.t=this->raySquareIntersection(p0,V);
    vec3 p03=vec3(p0.x,p0.y,p0.z);
    vec3 V3=vec3(V.x,V.y,V.z);
    vec3 p=p03+V3*result.t;


    //result.P=p0+V*result.t;
    result.P=vec4(p.x,p.y,p.z,1);



    result.ID_=1;
    result.N=this->normal;
    result.name=name;
    return result;
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */

double Square::raySquareIntersection(vec4 p0, vec4 V){
    double t   = std::numeric_limits< double >::infinity();
    //TODO: Ray-square intersection;
    vec3 point3=vec3(point.x,point.y,point.z);
    vec3 p03=vec3(p0.x,p0.y,p0.z);
    vec3 V3=vec3(V.x,V.y,V.z);


    double D=dot(point3,normal);
    double ON=dot(p03,normal);
    double dN=dot(V3,normal);

    if(dN!=0)
        t=((D)-ON)/dN;
    else
        t=std::numeric_limits< double >::infinity();
    if(t<0)
        return std::numeric_limits< double >::infinity();
    //x1*y2+y2*z2+z1*x2-y1*x2-x1*z2-z1*y2
    vec3 p=p03+V3*t;

    vec3 v1= vec3(mesh.vertices[0].x,mesh.vertices[0].y,mesh.vertices[0].z);
    vec3 v2= vec3(mesh.vertices[2].x,mesh.vertices[2].y,mesh.vertices[2].z);
    vec3 v3= vec3(mesh.vertices[1].x,mesh.vertices[1].y,mesh.vertices[1].z);
    vec3 v4= vec3(mesh.vertices[5].x,mesh.vertices[5].y,mesh.vertices[5].z);

    vec3 v1v2=v1-v2;
    vec3 v1p=v1-p;
    vec3 v12=cross(v1v2,v1p);
    double g1=dot(v12,normal);

    vec3 v2v3=v2-v3;
    vec3 v2p=v2-p;
    vec3 v23=cross(v2v3,v2p);
    double g2=dot(v23,normal);

    vec3 v3v4=v3-v4;
    vec3 v3p=v4-p;
    vec3 v34=cross(v3v4,v3p);
    double g3=dot(v34,normal);

    vec3 v4v1=v4-v1;
    vec3 v4p=v4-p;
    vec3 v41=cross(v4v1,v4p);
    double g4=dot(v41,normal);


    if(!((g1>=0)&&(g2>=0)&&(g3>=0)&&(g4>=0)))
        t=std::numeric_limits< double >::infinity();


    return t;
}

