//////////////////////////////////////////////////////////////////////////////
//
//  --- main.cpp ---
//  Created by Brian Summa
//
//////////////////////////////////////////////////////////////////////////////

#include "common.h"
#include "SourcePath.h"

using namespace Angel;

typedef vec4  color4;
typedef vec4  point4;


//Scene variables
enum{_SPHERE, _SQUARE, _BOX};
int scene = _SPHERE; //Simple sphere, square or cornell box
std::vector < Object * > sceneObjects;
point4 lightPosition;
color4 lightColor;
point4 cameraPosition;

//Recursion depth for raytracer
int maxDepth = 3;

void initGL();

namespace GLState {
    int window_width, window_height;

    bool render_line;

    std::vector < GLuint > objectVao;
    std::vector < GLuint > objectBuffer;

    GLuint vPosition, vNormal, vTexCoord;

    GLuint program;

    // Model-view and projection matrices uniform location
    GLuint  ModelView, ModelViewLight, NormalMatrix, Projection;

    //==========Trackball Variables==========
    static float curquat[4],lastquat[4];
    /* current transformation matrix */
    static float curmat[4][4];
    mat4 curmat_a;
    /* actual operation  */
    static int scaling;
    static int moving;
    static int panning;
    /* starting "moving" coordinates */
    static int beginx, beginy;
    /* ortho */
    float ortho_x, ortho_y;
    /* current scale factor */
    static float scalefactor;

    mat4  projection;
    mat4 sceneModelView;

    color4 light_ambient;
    color4 light_diffuse;
    color4 light_specular;


};

/* ------------------------------------------------------- */
/* -- PNG receptor class for use with pngdecode library -- */
class rayTraceReceptor : public cmps3120::png_receptor
{
private:
    const unsigned char *buffer;
    unsigned int width;
    unsigned int height;
    int channels;

public:
    rayTraceReceptor(const unsigned char *use_buffer,
                     unsigned int width,
                     unsigned int height,
                     int channels){
        this->buffer = use_buffer;
        this->width = width;
        this->height = height;
        this->channels = channels;
    }
    cmps3120::png_header get_header(){
        cmps3120::png_header header;
        header.width = width;
        header.height = height;
        header.bit_depth = 8;
        switch (channels)
        {
            case 1:
                header.color_type = cmps3120::PNG_GRAYSCALE;break;
            case 2:
                header.color_type = cmps3120::PNG_GRAYSCALE_ALPHA;break;
            case 3:
                header.color_type = cmps3120::PNG_RGB;break;
            default:
                header.color_type = cmps3120::PNG_RGBA;break;
        }
        return header;
    }
    cmps3120::png_pixel get_pixel(unsigned int x, unsigned int y, unsigned int level){
        cmps3120::png_pixel pixel;
        unsigned int idx = y*width+x;
        /* pngdecode wants 16-bit color values */
        pixel.r = buffer[4*idx]*257;
        pixel.g = buffer[4*idx+1]*257;
        pixel.b = buffer[4*idx+2]*257;
        pixel.a = buffer[4*idx+3]*257;
        return pixel;
    }
};

/* -------------------------------------------------------------------------- */
/* ----------------------  Write Image to Disk  ----------------------------- */
bool write_image(const char* filename, const unsigned char *Src,
                 int Width, int Height, int channels){
    cmps3120::png_encoder the_encoder;
    cmps3120::png_error result;
    rayTraceReceptor image(Src,Width,Height,channels);
    the_encoder.set_receptor(&image);
    result = the_encoder.write_file(filename);
    if (result == cmps3120::PNG_DONE)
        std::cerr << "finished writing "<<filename<<"."<<std::endl;
    else
        std::cerr << "write to "<<filename<<" returned error code "<<result<<"."<<std::endl;
    return result==cmps3120::PNG_DONE;
}


/* -------------------------------------------------------------------------- */
/* -------- Given OpenGL matrices find ray in world coordinates of ---------- */
/* -------- window position x,y --------------------------------------------- */
std::vector < vec4 > findRay(GLdouble x, GLdouble y){

    y = GLState::window_height-y;

    int viewport[4];
    glGetIntegerv(GL_VIEWPORT, viewport);

    GLdouble modelViewMatrix[16];
    GLdouble projectionMatrix[16];
    for(unsigned int i=0; i < 4; i++){
        for(unsigned int j=0; j < 4; j++){
            modelViewMatrix[j*4+i]  =  GLState::sceneModelView[i][j];
            projectionMatrix[j*4+i] =  GLState::projection[i][j];
        }
    }


    GLdouble nearPlaneLocation[3];
    _gluUnProject(x, y, 0.0, modelViewMatrix, projectionMatrix,
                  viewport, &nearPlaneLocation[0], &nearPlaneLocation[1],
                  &nearPlaneLocation[2]);

    GLdouble farPlaneLocation[3];
    _gluUnProject(x, y, 1.0, modelViewMatrix, projectionMatrix,
                  viewport, &farPlaneLocation[0], &farPlaneLocation[1],
                  &farPlaneLocation[2]);


    vec4 ray_origin = vec4(nearPlaneLocation[0], nearPlaneLocation[1], nearPlaneLocation[2], 1.0);
    vec3 temp = vec3(farPlaneLocation[0]-nearPlaneLocation[0],
                     farPlaneLocation[1]-nearPlaneLocation[1],
                     farPlaneLocation[2]-nearPlaneLocation[2]);
    temp = normalize(temp);
    vec4 ray_dir = vec4(temp.x, temp.y, temp.z, 0.0);

    std::vector < vec4 > result(2);
    result[0] = ray_origin;
    result[1] = ray_dir;

    return result;
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
bool intersectionSort(Object::IntersectionValues i, Object::IntersectionValues j){
    return (i.t < j.t);
}

/* -------------------------------------------------------------------------- */
/* ---------  Some debugging code: cast Ray = p0 + t*dir  ------------------- */
/* ---------  and print out what it hits =                ------------------- */
void castRayDebug(vec4 p0, vec4 dir){

    std::vector < Object::IntersectionValues > intersections;

    for(unsigned int i=0; i < sceneObjects.size(); i++){
        intersections.push_back(sceneObjects[i]->intersect(p0, dir));
        intersections[intersections.size()-1].ID_ = i;
    }

    for(unsigned int i=0; i < intersections.size(); i++){
        if(intersections[i].t != std::numeric_limits< double >::infinity()){
            std::cout << "Hit " << intersections[i].name << " " << intersections[i].ID_ << "\n";
            std::cout << "P: " <<  intersections[i].P << "\n";
            std::cout << "N: " <<  intersections[i].N << "\n";
            vec4 L = lightPosition-intersections[i].P;
            L  = normalize(L);
            std::cout << "L: " << L << "\n";
        }
    }

}

/* -------------------------------------------------------------------------- */


/* -------------------------------------------------------------------------- */
/* ----------  cast Ray = p0 + t*dir and intersect with sphere      --------- */
/* ----------  return color, right now shading is approx based      --------- */
/* ----------  depth                                                --------- */
// p0 : point de départ du rayon
// E : direction du rayon
// *lastHitObject : pointeur sur le dernier objet itersecter par le rayon
// depth : profondeur de recursion
vec4 castRay(vec4 p0, vec4 E, Object *lastHitObject, int depth){
    vec4 color = vec4(0.0,0.0,0.0,1.0);
    if (depth > maxDepth) return color;
    Object::IntersectionValues val2;
    Object::IntersectionValues val;
    std::vector < Object::IntersectionValues > intersections;
    Object * lastiHitobjectBuffer = lastHitObject;
    color4 shadow(0.0,0.0,0.0,1.0);
    double min = std::numeric_limits<double>::infinity();
    double dist = min;
    int firstindex=0;
    //////////////////////
    //////Intersect//////
    /////////////////////
    for (int i=0; i < sceneObjects.size(); i++) {
        val = sceneObjects[i]->intersect(p0, E);
        dist = val.t;

        if (dist< min && val.t>EPSILON) {
            min = dist;
            lastHitObject = sceneObjects[i];
            val2 = val;
            color = sceneObjects[i]->shadingValues.color;
            firstindex =i;

        }
    }

    //////////////////////
    ////////Phong////////
    /////////////////////
    if(min == std::numeric_limits<double>::infinity()) return color ;
    vec4 light = normalize(vec4(lightPosition-val2.P));
    light.w = 0.0;
    vec4 cam = normalize(vec4(cameraPosition-val2.P));
    vec4 vecnormal = normalize(vec4(val2.N));
    vecnormal.w = 0.0;
    vec4 dirayction = vec4(E);
    dirayction.w = 0.0;
    vec4 rayFlection = reflect(dirayction,vecnormal);
    rayFlection.w = 0.0;

    color4 material_ambient(lastHitObject->shadingValues.color.x*lastHitObject->shadingValues.Ka,
                            lastHitObject->shadingValues.color.y*lastHitObject->shadingValues.Ka,
                            lastHitObject->shadingValues.color.z*lastHitObject->shadingValues.Ka, 1.0 );

    color4 material_diffuse(lastHitObject->shadingValues.color.x*lastHitObject->shadingValues.Kd,
                            lastHitObject->shadingValues.color.y*lastHitObject->shadingValues.Kd,
                            lastHitObject->shadingValues.color.z*lastHitObject->shadingValues.Kd, 1.0 );

    color4 material_specular(lastHitObject->shadingValues.color.x*lastHitObject->shadingValues.Ks,
                             lastHitObject->shadingValues.color.y*lastHitObject->shadingValues.Ks,
                             lastHitObject->shadingValues.color.z*lastHitObject->shadingValues.Ks, 1.0 );



    float  material_shininess = lastHitObject->shadingValues.Kn;
    color4 ambient_product  = GLState::light_ambient * material_ambient;
    color4 diffuse_product  = GLState::light_diffuse * material_diffuse*std::max(0.0f,dot(light,vecnormal));
    color4 specular_product = GLState::light_specular * material_specular * pow ( std::min(0.0f,dot ((2 * dot(light,vecnormal)) * vecnormal - light, cam)),material_shininess);
    color = ambient_product + diffuse_product + specular_product;


    //////////////////////
    ///////OMBRE V1//////
    /////////////////////
     /*Object::IntersectionValues result;

     min=std::numeric_limits< double >::infinity();
     for(int i=0;i<sceneObjects.size();i++)
     {
                val=sceneObjects[i]->intersect(val2.P,normalize(-light));
                if(min>val.t)
                {
                    min=val.t;
                    index2=i;
                    result=val;
                }
     }


     float dis=sqrt(pow(val2.P.x-lightPosition.x,2)+pow(val2.P.y-lightPosition.y,2)+pow(val2.P.z-lightPosition.z,2));
     float dis2=sqrt(pow(val2.P.x-result.P.x,2)+pow(val2.P.y-result.P.y,2)+pow(val2.P.z-result.P.z,2));

    if((min<std::numeric_limits< double >::infinity())&&(firstindex!=index2)&&(dis>=dis2))
    {
        return shadow;
    }*/
    //////////////////////
    ///////OMBRE V2//////
    /////////////////////

    Object::IntersectionValues result;

    vec3 vec3ligt= vec3(val2.P.x-lightPosition.x,val2.P.y-lightPosition.y,val2.P.z-lightPosition.z);
    vec3 v1= vec3(-vec3ligt.y, vec3ligt.x, vec3ligt.z);
    vec3 v2= vec3((vec3ligt.y * v1.z) - (vec3ligt.z * v1.y), (vec3ligt.z * v1.x) - (vec3ligt.x * v1.z), (vec3ligt.x * v1.y) - (vec3ligt.y * v1.x));
    v1=normalize(v1);
    v2=normalize(v2);
    int sizematrix=3;


    vec3 matrixshadow[sizematrix][sizematrix];

    for(int i=-(sizematrix-1)/2;i<=(sizematrix-1)/2;i++)
        for(int j=-(sizematrix-1)/2;j<=(sizematrix-1)/2;j++)
        {
            matrixshadow[i+(sizematrix-1)/2][j+(sizematrix-1)/2]= vec3(lightPosition.x,lightPosition.y,lightPosition.z) + v1 * i * (1 / (float)sizematrix) + v2 * j * (1 / (float)sizematrix);

        }

    int numMatrix=0;
    for(int i=0;i<sizematrix;i++)
        for(int j=0;j<sizematrix;j++)
        {

            vec4 lightvec=vec4(matrixshadow[i][j].x-val2.P.x,matrixshadow[i][j].y-val2.P.y,matrixshadow[i][j].z-val2.P.z,0);
            Object::IntersectionValues result3;
            int index2=0;
            min=std::numeric_limits< double >::infinity();
            for(int ioo=0;ioo<sceneObjects.size();ioo++)
            {
                result=sceneObjects[ioo]->intersect(val2.P,normalize(-lightvec));
                if(min>result.t)
                {
                    min=result.t;
                    index2=ioo;
                    result3=result;
                }
            }
            float dis=sqrt(pow(val2.P.x-matrixshadow[i][j].x,2)+pow(val2.P.y-matrixshadow[i][j].y,2)+pow(val2.P.z-matrixshadow[i][j].z,2));
            float dis2=sqrt(pow(val2.P.x-result3.P.x,2)+pow(val2.P.y-result3.P.y,2)+pow(val2.P.z-result3.P.z,2));


            if((min<std::numeric_limits< double >::infinity())&&(firstindex!=index2)&&(dis>=dis2))
            {
                numMatrix++;
            }
        }

    color[3]=1.0;
    float maxi =1.0;
    for(int i=0;i<3;i++)
    {
        maxi=std::max(color[i],maxi);
    }
    color/=maxi;
    color[3]=1.0;

    for(int i=0;i<3;i++)
    {

        color[i]=color[i]-color[i]*(((float)numMatrix / (float)(sizematrix * sizematrix)));
    }
    for(int i=0;i<3;i++)
    {
        if(color[i]<0)
            color[i]=0;
    }

    //////////////////////
    //////REFRACTION/////
    /////////////////////

    double indice1;
    double indice2;
    if(lastHitObject->shadingValues.Kr > 0)
    {
        vec3 refractVec;
        vec3 normales;
        if(lastiHitobjectBuffer!=nullptr){

            if(lastHitObject->name ==lastiHitobjectBuffer->name)
            {
                indice1=1.000272;
                indice2=1.5;
                normales=-vec3(val2.N.x,val2.N.y,val2.N.z);

            }
            else
            {
                indice2=2;
                indice1=1.000272;

                normales=vec3(val2.N.x,val2.N.y,val2.N.z);

            }
        }
        else
        {
            indice2=2.5;
            indice1=1.000272;
            normales=vec3(val2.N.x,val2.N.y,val2.N.z);
        }
        double cos1=dot(normales,(-normalize(vec3(E.x,E.y,E.z))));
        double Theta=acos(cos1);
        double cos1carre=(1+cos(2*Theta))/2;
        double cos2=sqrt(abs(1-pow((indice1/indice2),2)*(1-cos1carre)));
        if(cos1>0)
            refractVec =normalize((indice1/indice2)*vec3(E.x,E.y,E.z)+((indice1/indice2)*cos1-cos2)*normales);
        else
            refractVec =normalize((indice1/indice2)*vec3(E.x,E.y,E.z)+((indice1/indice2)*cos1+cos2)*normales);


        color=castRay(val2.P,vec4(refractVec .x,refractVec .y,refractVec .z,0),lastHitObject,depth+1);

    }

    //////////////////////
    //////REFLEXION//////
    /////////////////////

    if(lastHitObject->shadingValues.Ks>=1){
        vec3 reflection = reflect(normalize(vec3(E.x,E.y,E.z)),normalize(vec3(val2.N.x,val2.N.y,val2.N.z)));
        color = (1-lastHitObject->shadingValues.Ks) * color + lastHitObject->shadingValues.Ks * castRay(val2.P,reflection,lastHitObject,depth+1);
    }

    color[3]=1.0;
    float maxi2 = 1.0;
    for (int i =0;i<3;i++){
        maxi2 = std::max(color[i],maxi2);
    }
    color/=maxi2;
    color[3]=1.0;

    return color;



}


/* -------------------------------------------------------------------------- */
/* ------------  Ray trace our scene.  Output color to image and    --------- */
/* -----------   Output color to image and save to disk             --------- */
void rayTrace(){

    unsigned char *buffer = new unsigned char[GLState::window_width*GLState::window_height*4];

    for(unsigned int i=0; i < GLState::window_width; i++){
        for(unsigned int j=0; j < GLState::window_height; j++){

            int idx = j*GLState::window_width+i;
            std::vector < vec4 > ray_o_dir = findRay(i,j);
            vec4 color = castRay(ray_o_dir[0], vec4(ray_o_dir[1].x, ray_o_dir[1].y, ray_o_dir[1].z,0.0), NULL, 0);
            buffer[4*idx]   = color.x*255;
            buffer[4*idx+1] = color.y*255;
            buffer[4*idx+2] = color.z*255;
            buffer[4*idx+3] = color.w*255;
        }
    }

    write_image("output.png", buffer, GLState::window_width, GLState::window_height, 4);

    delete[] buffer;
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
static void error_callback(int error, const char* description)
{
    fprintf(stderr, "Error: %s\n", description);
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void initCornellBox(){
    cameraPosition = point4( 0.0, 0.0, 6.0, 1.0 );
    lightPosition = point4( 0.0, 1.5, 0.0, 1.0 );
    lightColor = color4( 1.0, 1.0, 1.0, 1.0);

    sceneObjects.clear();
    {
        sceneObjects.push_back(new Sphere("Glass sphere", vec3(1.0, -1.25, 0.5),0.75));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(1.0,0.0,0.0,1.0);
        _shadingValues.Ka = .15;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 1.0;
        _shadingValues.Kr = 1.5;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    {
        sceneObjects.push_back(new Sphere("Mirrored Sphere", vec3(-1.0, -1.25, -0.5),0.75));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.0,0.0,1.0,1.0);
        _shadingValues.Ka = .15;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 1.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Back Wall
        sceneObjects.push_back(new Square("Back Wall", Translate(0.0, 0.0, -2.0)*Scale(2.0,2.0,1.0)));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.5,0.5,1.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Left Wall
        sceneObjects.push_back(new Square("Left Wall", RotateY(90)*Translate(0.0, 0.0, -2.0)*Scale(2.0,2.0,1.0)));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(1.0,0.0,0.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Right Wall
        sceneObjects.push_back(new Square("Right Wall", RotateY(-90)*Translate(0.0, 0.0, -2.0)*Scale(2.0, 2.0, 1.0 )));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.5,0.0,0.5,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Floor
        sceneObjects.push_back(new Square("Floor", RotateX(-90)*Translate(0.0, 0.0, -2.0)*Scale(2.0, 2.0, 1.0)));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.0,1.0,1.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Ceiling
        sceneObjects.push_back(new Square("Ceiling", RotateX(90)*Translate(0.0, 0.0, -2.0)*Scale(2.0, 2.0, 1.0)));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.4,0.0,0.6,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    { //Front Wall
        sceneObjects.push_back(new Square("Front Wall",RotateY(180)*Translate(0.0, 0.0, -2.0)*Scale(2.0, 2.0, 1.0)));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(1.0,0.2,0.8,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }



}


/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void initUnitSphere(){


    cameraPosition = point4( 0.0, 0.0, 3.0, 1.0 );
    lightPosition = point4( 0.0, 0.0, 4.0, 1.0 );
    lightColor = color4( 1.0, 1.0, 1.0, 1.0);

    sceneObjects.clear();


    {
        sceneObjects.push_back(new Sphere("sphere rouge", vec3(1.0, 0, 0),1.0));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(1.0,0.0,0.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 0.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 1.0;
        _shadingValues.Kr = 1.4;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

    {
        sceneObjects.push_back(new Sphere("sphere verte", vec3(-1.0, 0, 0),1.0));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.0,1.0,0.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 0.0;
        _shadingValues.Ks = 1.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

}



/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void initUnitSquare(){
    cameraPosition = point4( 0.0, 0.0, 3.0, 1.0 );
    lightPosition = point4( 0.0, 0.0, 4.0, 1.0 );
    lightColor = color4( 1.0, 1.0, 1.0, 1.0);

    sceneObjects.clear();

    { //Back Wall
        sceneObjects.push_back(new Square("Unit Square"));
        Object::ShadingValues _shadingValues;
        _shadingValues.color = vec4(0.0,1.0,1.0,1.0);
        _shadingValues.Ka = 0.0;
        _shadingValues.Kd = 1.0;
        _shadingValues.Ks = 0.0;
        _shadingValues.Kn = 16.0;
        _shadingValues.Kt = 0.0;
        _shadingValues.Kr = 0.0;
        sceneObjects[sceneObjects.size()-1]->setShadingValues(_shadingValues);
        sceneObjects[sceneObjects.size()-1]->setModelView(mat4());
    }

}


/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
static void keyCallback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
    if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
        glfwSetWindowShouldClose(window, GLFW_TRUE);
    if (key == GLFW_KEY_1 && action == GLFW_PRESS){

        if( scene != _SPHERE ){
            initUnitSphere();
            initGL();
            scene = _SPHERE;
        }

    }
    if (key == GLFW_KEY_2 && action == GLFW_PRESS){
        if( scene != _SQUARE ){
            initUnitSquare();
            initGL();
            scene = _SQUARE;
        }
    }
    if (key == GLFW_KEY_3 && action == GLFW_PRESS){
        if( scene != _BOX ){
            initCornellBox();
            initGL();
            scene = _BOX;
        }
    }


    if (key == GLFW_KEY_R && action == GLFW_PRESS)
        rayTrace();
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
static void mouseClick(GLFWwindow* window, int button, int action, int mods){

    if (GLFW_RELEASE == action){
        GLState::moving=GLState::scaling=GLState::panning=false;
        return;
    }

    if( mods & GLFW_MOD_SHIFT){
        GLState::scaling=true;
    }else if( mods & GLFW_MOD_ALT ){
        GLState::panning=true;
    }else{
        GLState::moving=true;
        TrackBall::trackball(GLState::lastquat, 0, 0, 0, 0);
    }

    double xpos, ypos;
    glfwGetCursorPos(window, &xpos, &ypos);
    GLState::beginx = xpos; GLState::beginy = ypos;

    std::vector < vec4 > ray_o_dir = findRay(xpos, ypos);
    castRayDebug(ray_o_dir[0], vec4(ray_o_dir[1].x, ray_o_dir[1].y, ray_o_dir[1].z,0.0));

}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void mouseMove(GLFWwindow* window, double x, double y){

    int W, H;
    glfwGetFramebufferSize(window, &W, &H);


    float dx=(x-GLState::beginx)/(float)W;
    float dy=(GLState::beginy-y)/(float)H;

    if (GLState::panning)
    {
        GLState::ortho_x  +=dx;
        GLState::ortho_y  +=dy;

        GLState::beginx = x; GLState::beginy = y;
        return;
    }
    else if (GLState::scaling)
    {
        GLState::scalefactor *= (1.0f+dx);

        GLState::beginx = x;GLState::beginy = y;
        return;
    }
    else if (GLState::moving)
    {
        TrackBall::trackball(GLState::lastquat,
                             (2.0f * GLState::beginx - W) / W,
                             (H - 2.0f * GLState::beginy) / H,
                             (2.0f * x - W) / W,
                             (H - 2.0f * y) / H
        );

        TrackBall::add_quats(GLState::lastquat, GLState::curquat, GLState::curquat);
        TrackBall::build_rotmatrix(GLState::curmat, GLState::curquat);

        GLState::beginx = x;GLState::beginy = y;
        return;
    }
}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void initGL(){

    GLState::light_ambient  = vec4(lightColor.x, lightColor.y, lightColor.z, 1.0 );
    GLState::light_diffuse  = vec4(lightColor.x, lightColor.y, lightColor.z, 1.0 );
    GLState::light_specular = vec4(lightColor.x, lightColor.y, lightColor.z, 1.0 );


    std::string vshader = source_path + "/shaders/vshader.glsl";
    std::string fshader = source_path + "/shaders/fshader.glsl";

    GLchar* vertex_shader_source = readShaderSource(vshader.c_str());
    GLchar* fragment_shader_source = readShaderSource(fshader.c_str());

    GLuint vertex_shader = glCreateShader(GL_VERTEX_SHADER);
    glShaderSource(vertex_shader, 1, (const GLchar**) &vertex_shader_source, NULL);
    glCompileShader(vertex_shader);
    check_shader_compilation(vshader, vertex_shader);

    GLuint fragment_shader = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(fragment_shader, 1, (const GLchar**) &fragment_shader_source, NULL);
    glCompileShader(fragment_shader);
    check_shader_compilation(fshader, fragment_shader);

    GLState::program = glCreateProgram();
    glAttachShader(GLState::program, vertex_shader);
    glAttachShader(GLState::program, fragment_shader);

    glLinkProgram(GLState::program);
    check_program_link(GLState::program);

    glUseProgram(GLState::program);

    glBindFragDataLocation(GLState::program, 0, "fragColor");

    // set up vertex arrays
    GLState::vPosition = glGetAttribLocation( GLState::program, "vPosition" );
    GLState::vNormal = glGetAttribLocation( GLState::program, "vNormal" );

    // Retrieve transformation uniform variable locations
    GLState::ModelView = glGetUniformLocation( GLState::program, "ModelView" );
    GLState::NormalMatrix = glGetUniformLocation( GLState::program, "NormalMatrix" );
    GLState::ModelViewLight = glGetUniformLocation( GLState::program, "ModelViewLight" );
    GLState::Projection = glGetUniformLocation( GLState::program, "Projection" );

    GLState::objectVao.resize(sceneObjects.size());
    glGenVertexArrays( sceneObjects.size(), &GLState::objectVao[0] );

    GLState::objectBuffer.resize(sceneObjects.size());
    glGenBuffers( sceneObjects.size(), &GLState::objectBuffer[0] );

    for(unsigned int i=0; i < sceneObjects.size(); i++){
        glBindVertexArray( GLState::objectVao[i] );
        glBindBuffer( GL_ARRAY_BUFFER, GLState::objectBuffer[i] );
        size_t vertices_bytes = sceneObjects[i]->mesh.vertices.size()*sizeof(vec4);
        size_t normals_bytes  =sceneObjects[i]->mesh.normals.size()*sizeof(vec3);

        glBufferData( GL_ARRAY_BUFFER, vertices_bytes + normals_bytes, NULL, GL_STATIC_DRAW );
        size_t offset = 0;
        glBufferSubData( GL_ARRAY_BUFFER, offset, vertices_bytes, &sceneObjects[i]->mesh.vertices[0] );
        offset += vertices_bytes;
        glBufferSubData( GL_ARRAY_BUFFER, offset, normals_bytes,  &sceneObjects[i]->mesh.normals[0] );

        glEnableVertexAttribArray( GLState::vNormal );
        glEnableVertexAttribArray( GLState::vPosition );

        glVertexAttribPointer( GLState::vPosition, 4, GL_FLOAT, GL_FALSE, 0, BUFFER_OFFSET(0) );
        glVertexAttribPointer( GLState::vNormal, 3, GL_FLOAT, GL_FALSE, 0, BUFFER_OFFSET(vertices_bytes));

    }



    glEnable( GL_DEPTH_TEST );
    glShadeModel(GL_SMOOTH);

    glClearColor( 0.8, 0.8, 1.0, 1.0 );

    //Quaternion trackball variables, you can ignore
    GLState::scaling  = 0;
    GLState::moving   = 0;
    GLState::panning  = 0;
    GLState::beginx   = 0;
    GLState::beginy   = 0;

    TrackBall::matident(GLState::curmat);
    TrackBall::trackball(GLState::curquat , 0.0f, 0.0f, 0.0f, 0.0f);
    TrackBall::trackball(GLState::lastquat, 0.0f, 0.0f, 0.0f, 0.0f);
    TrackBall::add_quats(GLState::lastquat, GLState::curquat, GLState::curquat);
    TrackBall::build_rotmatrix(GLState::curmat, GLState::curquat);

    GLState::scalefactor = 1.0;
    GLState::render_line = false;

}

/* -------------------------------------------------------------------------- */
/* -------------------------------------------------------------------------- */
void drawObject(Object * object, GLuint vao, GLuint buffer){

    color4 material_ambient(object->shadingValues.color.x*object->shadingValues.Ka,
                            object->shadingValues.color.y*object->shadingValues.Ka,
                            object->shadingValues.color.z*object->shadingValues.Ka, 1.0 );
    color4 material_diffuse(object->shadingValues.color.x,
                            object->shadingValues.color.y,
                            object->shadingValues.color.z, 1.0 );
    color4 material_specular(object->shadingValues.Ks,
                             object->shadingValues.Ks,
                             object->shadingValues.Ks, 1.0 );
    float  material_shininess = object->shadingValues.Kn;

    color4 ambient_product  = GLState::light_ambient * material_ambient;
    color4 diffuse_product  = GLState::light_diffuse * material_diffuse;
    color4 specular_product = GLState::light_specular * material_specular;

    glUniform4fv( glGetUniformLocation(GLState::program, "AmbientProduct"), 1, ambient_product );
    glUniform4fv( glGetUniformLocation(GLState::program, "DiffuseProduct"), 1, diffuse_product );
    glUniform4fv( glGetUniformLocation(GLState::program, "SpecularProduct"), 1, specular_product );
    glUniform4fv( glGetUniformLocation(GLState::program, "LightPosition"), 1, lightPosition );
    glUniform1f(  glGetUniformLocation(GLState::program, "Shininess"), material_shininess );

    glBindVertexArray(vao);
    glBindBuffer( GL_ARRAY_BUFFER, buffer );
    glVertexAttribPointer( GLState::vPosition, 4, GL_FLOAT, GL_FALSE, 0, BUFFER_OFFSET(0) );
    glVertexAttribPointer( GLState::vNormal, 3, GL_FLOAT, GL_FALSE, 0, BUFFER_OFFSET(object->mesh.vertices.size()*sizeof(vec4)) );

    mat4 objectModelView = GLState::sceneModelView*object->getModelView();


    glUniformMatrix4fv( GLState::ModelViewLight, 1, GL_TRUE, GLState::sceneModelView);
    glUniformMatrix3fv( GLState::NormalMatrix, 1, GL_TRUE, Normal(objectModelView));
    glUniformMatrix4fv( GLState::ModelView, 1, GL_TRUE, objectModelView);

    glDrawArrays( GL_TRIANGLES, 0, object->mesh.vertices.size() );

}


int main(void){

    GLFWwindow* window;

    glfwSetErrorCallback(error_callback);

    if (!glfwInit())
        exit(EXIT_FAILURE);

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 2);
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    glfwWindowHint(GLFW_SAMPLES, 4);

    window = glfwCreateWindow(768, 768, "Raytracer", NULL, NULL);
    if (!window){
        glfwTerminate();
        exit(EXIT_FAILURE);
    }

    glfwSetKeyCallback(window, keyCallback);
    glfwSetMouseButtonCallback(window, mouseClick);
    glfwSetCursorPosCallback(window, mouseMove);


    glfwMakeContextCurrent(window);
    gladLoadGLLoader((GLADloadproc) glfwGetProcAddress);
    glfwSwapInterval(1);

    switch(scene){
        case _SPHERE:
            initUnitSphere();
            break;
        case _SQUARE:
            initUnitSquare();
            break;
        case _BOX:
            initCornellBox();
            break;
    }

    initGL();

    while (!glfwWindowShouldClose(window)){

        int width, height;
        glfwGetFramebufferSize(window, &width, &height);

        GLState::window_height = height;
        GLState::window_width  = width;

        glViewport(0, 0, width, height);


        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        mat4 track_ball =  mat4(GLState::curmat[0][0], GLState::curmat[1][0],
                                GLState::curmat[2][0], GLState::curmat[3][0],
                                GLState::curmat[0][1], GLState::curmat[1][1],
                                GLState::curmat[2][1], GLState::curmat[3][1],
                                GLState::curmat[0][2], GLState::curmat[1][2],
                                GLState::curmat[2][2], GLState::curmat[3][2],
                                GLState::curmat[0][3], GLState::curmat[1][3],
                                GLState::curmat[2][3], GLState::curmat[3][3]);

        GLState::sceneModelView  =  Translate(-cameraPosition) *   //Move Camera Back
                                    Translate(GLState::ortho_x, GLState::ortho_y, 0.0) *
                                    track_ball *                   //Rotate Camera
                                    Scale(GLState::scalefactor,
                                          GLState::scalefactor,
                                          GLState::scalefactor);   //User Scale

        GLfloat aspect = GLfloat(width)/height;

        switch(scene){
            case _SPHERE:
            case _SQUARE:
                GLState::projection = Perspective( 45.0, aspect, 0.01, 100.0 );
                break;
            case _BOX:
                GLState::projection = Perspective( 45.0, aspect, 4.5, 100.0 );
                break;
        }

        glUniformMatrix4fv( GLState::Projection, 1, GL_TRUE, GLState::projection);

        for(unsigned int i=0; i < sceneObjects.size(); i++){
            drawObject(sceneObjects[i], GLState::objectVao[i], GLState::objectBuffer[i]);
        }

        glfwSwapBuffers(window);
        glfwPollEvents();

    }

    glfwDestroyWindow(window);

    glfwTerminate();
    exit(EXIT_SUCCESS);
}
