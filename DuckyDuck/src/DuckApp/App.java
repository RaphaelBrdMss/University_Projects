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



        ////////////////////////////////////
        ////Init Grid, Duckpos & Foodpos////
        ///////////////////////////////////
        Grid g = new Grid(20,8);
        Ducky duck = new Ducky(g.size,g);
        Hunter hunter = new Hunter(g.size,g,1);
        // instantiate food
        Food Fish = new Food(g.size,g);
        for(int i =0 ; i<hunter.fov.size() ; i++){
            System.out.println(hunter.fov.get(i).toString());
        }




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

        String OsType = System.getProperty("os.name").toLowerCase();

        if(OsType.contains("mac") || OsType.contains("nix") || OsType.contains("nux") || OsType.contains("aix") ) {

            input = new FileInputStream("Texture/Ducky.png");
            inputFish= new FileInputStream("Texture/Truite.png");
            inputGrass = new FileInputStream("Texture/Herbe.jpg");
            inputWater = new FileInputStream("Texture/Eau.jpg");
            inputRoseau = new FileInputStream("Texture/Roseau.jpg");
            inputHunter = new FileInputStream("Texture/Chasseur.png");
            inputDeadDucky = new FileInputStream("Texture/DeadDucky.png");



        }else {
            input = new FileInputStream("Texture\\Ducky.png");
            inputFish = new FileInputStream("Texture\\Truite.png");
            inputGrass = new FileInputStream("Texture\\Herbe.jpg");
            inputWater = new FileInputStream("Texture\\Eau.jpg");
            inputRoseau = new FileInputStream("Texture\\Roseau.jpg");
            inputHunter = new FileInputStream("Texture\\Chasseur.png");
            inputDeadDucky = new FileInputStream("Texture\\DeadDucky.png");
        }
        Image image = new Image(input);
        ImageView duckyPNG = new ImageView(image);
        duckyPNG.relocate(duck.pos.x*scaleCell,duck.pos.y*scaleCell);

        Image imageFish = new Image(inputFish);
        ImageView TruitePNG = new ImageView(imageFish);
        TruitePNG.relocate(Fish.pos.x*scaleCell,Fish.pos.y*scaleCell);

        Image imageHunter = new Image(inputHunter);
        ImageView HunterPNG = new ImageView(imageHunter);
        HunterPNG.relocate(hunter.pos.x *scaleCell, hunter.pos.y *scaleCell);

        Image imageDead = new Image(inputDeadDucky);




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
        fondMbar.setWidth(widthBar*duck.getEstomac());
        mangerbar.setWidth(widthBar*duck.getEstomac());
        fondMbar.setX(50.0);
        fondMbar.setY(50.0);
        mangerbar.setX(50.0);
        mangerbar.setY(50.0);

        // Energie
        Rectangle fondStaminaBar = new Rectangle(widthBar, 50.0, Color.BLACK);
        Rectangle staminaBar = new Rectangle(widthBar, 50.0, Color.YELLOW);
        fondStaminaBar.setWidth(widthBar*duck.getM_stamina());
        staminaBar.setWidth(widthBar*duck.getM_stamina());
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
                if (duck.getM_state() == StateHero.EATING) {
                    boolean isSameFish = Fish.isId(duck.getEatenId());
                    if (isSameFish) {
                        // eat - relocate
                        Fish.regenerate(g.size, g);
                        TruitePNG.relocate(Fish.pos.x * scaleCell, Fish.pos.y * scaleCell);
                    }
                }


                //----------------
                // HUNTER STATE
                //----------------

                Cell prevCell = g.getCell(hunter.pos.x, hunter.pos.y);
                hunter.Update(duck.pos);
                Cell nextCell = g.getCell(hunter.pos.x, hunter.pos.y);
                if (nextCell.type == GroundType.GROUND) {

                    HunterPNG.relocate(hunter.pos.x * scaleCell, hunter.pos.y * scaleCell);
                    hunter.setFov(g.getFov(hunter.pos, 3));

                } else {

                    hunter.setPos(prevCell);

                }


                // ---------------
                // DUCKY STATE
                // ---------------

                duck.Update();

                if (hunter.shooted) {
                    duck.isShooted = true;
                }

                if (duck.getM_state() != StateHero.DEAD) {

                    duckyPNG.relocate(duck.pos.x * scaleCell, duck.pos.y * scaleCell);
                } else {

                    duckyPNG.setImage(imageDead);
                }

                duck.setFov(g.getFov(duck.pos, 2));
                // if in water => send the nearest fish
                if (duck.inWater) {
                    // determine which food : m_id
                    duck.setFoodWater(Fish.pos, Fish.getId());
                }


                // ---------------
                // User Information
                // ---------------
                mangerbar.setWidth(widthBar * duck.getEstomac());
                staminaBar.setWidth(widthBar * duck.getM_stamina());

            }

        });


        //add Ducky & food to the scene (after mouvement)
        root.getChildren().add(duckyPNG);
        root.getChildren().add(TruitePNG);
        root.getChildren().add(fondMbar);
        root.getChildren().add(mangerbar);
        root.getChildren().add(fondStaminaBar);
        root.getChildren().add(staminaBar);
        root.getChildren().add(HunterPNG);





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


