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
import javafx.scene.layout.BorderPane;
import javafx.scene.layout.Pane;
import javafx.scene.layout.Region;
import javafx.stage.Stage;

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
        Scene scene = new Scene(bdPane, 1000, 1000);

        ////////////////////////////////////
        ////Init Grid, Duckpos & Foodpos////
        ///////////////////////////////////
        Grid g = new Grid(20,5);
        Ducky duck = new Ducky(g.size,g);
        FileInputStream input = new FileInputStream("Ducky.png");
        Image image = new Image(input);
        ImageView duckyPNG = new ImageView(image);
        duckyPNG.relocate(duck.pos.x*100,duck.pos.y*100);

        Food Fish = new Food(g.size,g);
        FileInputStream inputFish = new FileInputStream("Truite.png");
        Image imageFish = new Image(inputFish);
        ImageView TruitePNG = new ImageView(imageFish);
        TruitePNG.relocate(Fish.pos.x*100,Fish.pos.y*100);




/*---------------------------------------------------------------------------------
Init cells
 ---------------------------------------------------------------------------------*/

        for(Cell c : g.CellsList){

            Region cellshow = new Region();
            int x= 100*c.x;
            int y = 100*c.y;


            if(c.type == GroundType.GROUND){

                cellshow.setStyle("-fx-background-color: green;" +
                        "-fx-min-width: 100; -fx-min-height: 100; -fx-max-width: 100; -fx-max-height: 100;");
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);
            }

            if(c.type == GroundType.WATER){

                cellshow.setStyle("-fx-background-color: blue;" +
                        "-fx-min-width: 100; -fx-min-height: 100; -fx-max-width: 100; -fx-max-height: 100;");
                cellshow.setLayoutX(x);
                cellshow.setLayoutY(y);
            }

            root.getChildren().add(cellshow);

        }

/*---------------------------------------------------------------------------------
End init cells
 ---------------------------------------------------------------------------------*/

        /////////////////////////////////////////
        ////Movement pressing A key (azerty)////
        ////////////////////////////////////////
        scene.setOnKeyPressed(e -> {
            if (e.getCode() == KeyCode.Q) {
                duck.move();
                duckyPNG.relocate(duck.pos.x*100,duck.pos.y*100);
                duck.setFov(g.getFov(duck.pos));

            }
        });



        //add Ducky & food to the scene (after mouvement)
        root.getChildren().add(duckyPNG);
        root.getChildren().add(TruitePNG);


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


