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

        ////////////////////////////////////
        ////Init Grid, Duckpos & Foodpos////
        ///////////////////////////////////
        Grid g = new Grid(20,8);
        Ducky duck = new Ducky(g.size,g);
        // instantiate food
        Food Fish = new Food(g.size,g);

        

        //////////////////////
        ////Init Texture ////
        /////////////////////
        FileInputStream input;
        FileInputStream inputFish;
        FileInputStream inputGrass;
        FileInputStream inputWater;

        String OsType = System.getProperty("os.name").toLowerCase();

        if(OsType.contains("mac") || OsType.contains("nix") || OsType.contains("nux") || OsType.contains("aix") ) {

            input = new FileInputStream("Texture/Ducky.png");
            inputFish= new FileInputStream("Texture/Truite.png");
            inputGrass = new FileInputStream("Texture/Herbe.jpg");
            inputWater = new FileInputStream("TeXture/Eau.jpg");

        }else {
             input = new FileInputStream("Texture\\Ducky.png");
             inputFish = new FileInputStream("Texture\\Truite.png");
             inputGrass = new FileInputStream("Texture\\Herbe.jpg");
             inputWater = new FileInputStream("TeXture\\Eau.jpg");
        }
        Image image = new Image(input);
        ImageView duckyPNG = new ImageView(image);
        duckyPNG.relocate(duck.pos.x*100,duck.pos.y*100);

        Image imageFish = new Image(inputFish);
        ImageView TruitePNG = new ImageView(imageFish);
        TruitePNG.relocate(Fish.pos.x*100,Fish.pos.y*100);

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

        for(Cell c : g.CellsList){

            Region cellshow = new Region();
            int x= 100*c.x;
            int y = 100*c.y;


            if(c.type == GroundType.GROUND){





                cellshow.setStyle("-fx-background-image : url ('Texture/Herbe.jpg');" +
                        "-fx-min-width: 100; -fx-min-height: 100; -fx-max-width: 100; -fx-max-height: 100;");
                cellshow.setBackground(grass);
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);
            }

            if(c.type == GroundType.WATER){

                cellshow.setStyle("-fx-background-image : url ('Texture/Eau.jpg');" +
                        "-fx-min-width: 100; -fx-min-height: 100; -fx-max-width: 100; -fx-max-height: 100;");
                cellshow.setBackground(water);
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
        double widthManger = 2.0;
        Rectangle fondMbar = new Rectangle(widthManger, 50.0, Color.BLACK);
        Rectangle mangerbar = new Rectangle(widthManger, 50.0, Color.RED);
        fondMbar.setWidth(widthManger*duck.estomac);
        mangerbar.setWidth(widthManger*duck.estomac);
        fondMbar.setX(50.0);
        fondMbar.setY(50.0);
        mangerbar.setX(50.0);
        mangerbar.setY(50.0);


        /////////////////////////////////////////
        ////Movement pressing A key (azerty)////
        ////////////////////////////////////////
        scene.setOnKeyPressed(e -> {
            if (e.getCode() == KeyCode.Q) {

                // ---------------
                // FOOD HANDLING
                // ---------------
                if (duck.m_state == StateHero.EATING)
                {
                    boolean isSameFish = Fish.isId(duck.getEatenId());
                    if (isSameFish)
                    {
                        // eat - relocate
                        Fish.regenerate(g.size,g);
                        TruitePNG.relocate(Fish.pos.x*100.0, Fish.pos.y*100.0);
                    }
                }

                // ---------------
                // DUCKY STATE
                // ---------------
                duck.Update();
                duckyPNG.relocate(duck.pos.x*100,duck.pos.y*100);
                duck.setFov(g.getFov(duck.pos));
                // if in water => send the nearest fish
                if (duck.inWater)
                {
                    // determine which food : m_id
                    duck.setFoodWater(Fish.pos, Fish.getId());
                }

                // ---------------
                // User Information
                // ---------------
                mangerbar.setWidth(widthManger*duck.estomac);

            }
        });


        //add Ducky & food to the scene (after mouvement)
        root.getChildren().add(duckyPNG);
        root.getChildren().add(TruitePNG);
        root.getChildren().add(fondMbar);
        root.getChildren().add(mangerbar);


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


