package DuckApp;

import javafx.application.Application;
import javafx.event.EventHandler;
import javafx.scene.Group;
import javafx.scene.Scene;
import javafx.scene.control.ScrollPane;
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.input.KeyCode;
import javafx.scene.input.ScrollEvent;
import javafx.scene.layout.*;
import javafx.scene.paint.Color;
import javafx.stage.Stage;
// bars
import javafx.scene.shape.Rectangle;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.util.ArrayList;

public class App extends Application {


    @Override
    public void start(Stage stage) throws FileNotFoundException {

        ////////////////////
        ////Init JavaFx////
        ///////////////////
        Pane root = new Pane();
        BorderPane bdPane = new BorderPane();
        ScrollPane scroll = new ScrollPane(new Group(root));
        bdPane.setCenter(scroll);
        Scene scene = new Scene(bdPane, 800, 800);
        int scaleCell = 200;



        ////////////////////////////
        ////Init Grid & Foodpos////
        //////////////////////////
        int nbDuck = 9;
        int nbHunter = 3;
        Grid g = new Grid(20,8);

        // instantiate food
        Food Fish = new Food(g.size,g);




        //////////////////////
        ////Init Texture ////
        /////////////////////
        FileInputStream input;
        FileInputStream inputFish;
        FileInputStream inputGrass;
        FileInputStream inputWater;
        FileInputStream inputRoseau;
        FileInputStream inputHunter;
        FileInputStream inputDeadDucky;
        FileInputStream inputDuck;
        FileInputStream inputDeadDuck;

        String OsType = System.getProperty("os.name").toLowerCase();

        if(OsType.contains("mac") || OsType.contains("nix") || OsType.contains("nux") || OsType.contains("aix") ) {

            input = new FileInputStream("Texture/Ducky.png");
            inputFish= new FileInputStream("Texture/Truite.png");
            inputGrass = new FileInputStream("Texture/Herbe.jpg");
            inputWater = new FileInputStream("Texture/Eau.jpg");
            inputRoseau = new FileInputStream("Texture/Roseau.jpg");
            inputHunter = new FileInputStream("Texture/Chasseur.png");
            inputDeadDucky = new FileInputStream("Texture/DeadDucky.png");
            inputDuck = new FileInputStream("Texture/Duck.png");
            inputDeadDuck = new FileInputStream("Texture/DeadDuck.png");




        }else {
            input = new FileInputStream("Texture\\Ducky.png");
            inputFish = new FileInputStream("Texture\\Truite.png");
            inputGrass = new FileInputStream("Texture\\Herbe.jpg");
            inputWater = new FileInputStream("Texture\\Eau.jpg");
            inputRoseau = new FileInputStream("Texture\\Roseau.jpg");
            inputHunter = new FileInputStream("Texture\\Chasseur.png");
            inputDeadDucky = new FileInputStream("Texture\\DeadDucky.png");
            inputDuck = new FileInputStream("Texture\\Duck.png");
            inputDeadDuck = new FileInputStream("Texture\\DeadDuck.png");
        }

        Image image = new Image(input);
        ImageView duckyPNG = new ImageView(image);
        Image Duckimage = new Image(inputDuck);

        ArrayList<ImageView>  DucksPNG = new ArrayList<>();
        ArrayList<Ducky> ducks = new ArrayList<>();
        //Init canard
        Ducky ducky = new Ducky(g.size,g);
        ducks.add(ducky);
        duckyPNG.relocate(ducky.pos.x*scaleCell,ducky.pos.y*scaleCell);
        DucksPNG.add(duckyPNG);

        for(int i = 0 ; i<nbDuck; i++){
            Ducky duck = new Ducky(g.size,g);
            ducks.add(duck);
            ImageView DuckPNG = new ImageView( Duckimage);
            DuckPNG.relocate(duck.pos.x * scaleCell, duck.pos.y *scaleCell);
            DucksPNG.add(DuckPNG);

        }





        // Init hunters
        ArrayList<Hunter> hunters = new ArrayList<>();
        Image imageHunter = new Image(inputHunter);
        ArrayList<ImageView> HuntersPNG = new ArrayList<>();
        for(int i = 0 ; i<nbHunter ; i++){
            Hunter hunter = new Hunter(g.size,g,1);
            hunters.add(hunter);
            ImageView HunterPNG = new ImageView(imageHunter);
            HunterPNG.relocate(hunter.pos.x * scaleCell, hunter.pos.y * scaleCell);
            HuntersPNG.add(HunterPNG);
        }


        Image imageFish = new Image(inputFish);
        ImageView TruitePNG = new ImageView(imageFish);
        TruitePNG.relocate(Fish.pos.x*scaleCell,Fish.pos.y*scaleCell);


        Image imageDead = new Image(inputDeadDucky);
        Image deadDuck  = new Image(inputDeadDuck);




        Image imageRoseau = new Image(inputRoseau);
        BackgroundImage bRoseau = new BackgroundImage(imageRoseau,
                BackgroundRepeat.REPEAT,
                BackgroundRepeat.REPEAT,
                BackgroundPosition.CENTER,
                BackgroundSize.DEFAULT);
        Background roseau = new Background(bRoseau);




        Image imageGrass = new Image(inputGrass);
        BackgroundImage bGrass = new BackgroundImage(imageGrass,
                BackgroundRepeat.REPEAT,
                BackgroundRepeat.REPEAT,
                BackgroundPosition.CENTER,
                BackgroundSize.DEFAULT);
        Background grass = new Background(bGrass);


        Image imageWater = new Image(inputWater);
        BackgroundImage bWater = new BackgroundImage(imageWater,
                BackgroundRepeat.REPEAT,
                BackgroundRepeat.REPEAT,
                BackgroundPosition.CENTER,
                BackgroundSize.DEFAULT);
        Background water = new Background(bWater);









/*---------------------------------------------------------------------------------
Init cells
 ---------------------------------------------------------------------------------*/
        String setStyle = "-fx-min-width:" +scaleCell+ "; -fx-min-height: "+scaleCell+"; -fx-max-width: " +scaleCell+
                "; -fx-max-height: "+scaleCell+";";
        for(Cell c : g.CellsList){

            Region cellshow = new Region();
            int x= scaleCell*c.x;
            int y = scaleCell*c.y;


            if(c.type == GroundType.GROUND){


                cellshow.setStyle(setStyle);
                cellshow.setBackground(grass);
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);
            }

            if(c.type == GroundType.WATER){

                cellshow.setStyle(setStyle);
                cellshow.setBackground(water);
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);
            }

            if(c.type == GroundType.ROSEAU){
                cellshow.setStyle(setStyle);
                cellshow.setBackground(roseau);
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);


            }

            root.getChildren().add(cellshow);


        }







/*---------------------------------------------------------------------------------
End init cells
 ---------------------------------------------------------------------------------*/
        // ---------------
        // BARS
        // ---------------
        // Manger
        double widthBar = 2.0;
        Rectangle fondMbar = new Rectangle(widthBar, 50.0, Color.BLACK);
        Rectangle mangerbar = new Rectangle(widthBar, 50.0, Color.RED);
        fondMbar.setWidth(widthBar*ducky.getEstomac());
        mangerbar.setWidth(widthBar*ducky.getEstomac());
        fondMbar.setX(50.0);
        fondMbar.setY(50.0);
        mangerbar.setX(50.0);
        mangerbar.setY(50.0);

        // Energie
        Rectangle fondStaminaBar = new Rectangle(widthBar, 50.0, Color.BLACK);
        Rectangle staminaBar = new Rectangle(widthBar, 50.0, Color.YELLOW);
        fondStaminaBar.setWidth(widthBar*ducky.getM_stamina());
        staminaBar.setWidth(widthBar*ducky.getM_stamina());
        fondStaminaBar.setX(50.0);
        fondStaminaBar.setY(50.0 + 80.0);
        staminaBar.setX(50.0);
        staminaBar.setY(50.0 + 80.0);


        /////////////////////////////////////////
        ////Movement pressing A key (azerty)////
        ////////////////////////////////////////
        scene.setOnKeyPressed(e -> {
            if (e.getCode() == KeyCode.Q) {

                // ---------------
                // FOOD HANDLING
                // ---------------



                //----------------
                // HUNTERS STATE
                //----------------
                int itr = 0;
                for(Hunter hunter : hunters) {

                    Cell prevCell = g.getCell(hunter.pos.x, hunter.pos.y);
                    hunter.Update(ducks);
                    Cell nextCell = g.getCell(hunter.pos.x, hunter.pos.y);
                    if (nextCell.type == GroundType.GROUND) {

                        HuntersPNG.get(itr).relocate(hunter.pos.x * scaleCell, hunter.pos.y * scaleCell);
                        hunter.setFov(g.getFov(hunter.pos, 3));

                    } else {

                        hunter.setPos(prevCell);

                    }
                    itr++;
                }


                // ---------------
                // DUCKY STATE
                // ---------------


                int itrD = 0;

                for (Ducky duck : ducks) {
                    if (duck.getM_state() == StateHero.EATING) {
                        boolean isSameFish = Fish.isId(duck.getEatenId());
                        if (isSameFish) {
                            // eat - relocate
                            Fish.regenerate(g.size, g);
                            TruitePNG.relocate(Fish.pos.x * scaleCell, Fish.pos.y * scaleCell);
                        }
                    }



                    for(Hunter h : hunters){
                        if (duck.equals(h.target) && h.shootSucced) {
                            duck.isShooted = true;
                            break;
                        }
                    }


                    duck.Update();
                    if (duck.getM_state() != StateHero.DEAD) {


                        DucksPNG.get(itrD).relocate(duck.pos.x * scaleCell, duck.pos.y * scaleCell);
                    } else {
                        for (Hunter h: hunters) {

                            if (itrD == 0) {
                                DucksPNG.get(itrD).setImage(imageDead);
                            } else {
                                DucksPNG.get(itrD).setImage(deadDuck);
                                if(h.pos.equals(duck.pos)){
                                    System.out.println("remove iamges");
                                    ducks.remove(duck);
                                    DucksPNG.get(itrD).setImage(null);

                                    DucksPNG.remove(itrD);
                                }

                            }
                        }
                    }

                    duck.setFov(g.getFov(duck.pos, 2));
                    // if in water => send the nearest fish
                    if (duck.inWater) {
                        // determine which food : m_id
                        duck.setFoodWater(Fish.pos, Fish.getId());
                    }
                    itrD++;
                }
                // ---------------
                // User Information
                // ---------------
                mangerbar.setWidth(widthBar * ducky.getEstomac());
                staminaBar.setWidth(widthBar * ducky.getM_stamina());

            }

        });


        //add Ducky & food to the scene (after mouvement)

        root.getChildren().add(TruitePNG);
        root.getChildren().add(fondMbar);
        root.getChildren().add(mangerbar);
        root.getChildren().add(fondStaminaBar);
        root.getChildren().add(staminaBar);
        for(ImageView Dpng : DucksPNG){
            root.getChildren().add(Dpng);
        }
        for(ImageView Hpng : HuntersPNG ) {
            root.getChildren().add(Hpng);
        }






        //zooming fonction
        this.zooming(root);

        stage.setScene(scene);
        stage.show();
    }


    //zoom fonction
    public void zooming (Pane pane){
        pane.setOnScroll(
                new EventHandler<ScrollEvent>() {
                    @Override
                    public void handle(ScrollEvent scrollEvent) {
                        double factor = 1.05;
                        double deltay = scrollEvent.getDeltaY();
                        if (deltay <0){
                            factor = 0.95;
                        }
                        pane.setScaleX(pane.getScaleX()* factor);
                        pane.setScaleY(pane.getScaleY()*factor);
                        scrollEvent.consume();

                    }
                }
        );
    }



    public static void main(String[] args) {
        launch(args);
    }
}


