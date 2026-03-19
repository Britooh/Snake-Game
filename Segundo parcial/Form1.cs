using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging; //añade esto para guardar la imagen
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Segundo_parcial
{
    public partial class Form1 : Form
    {
        //crea una lista de tipo Circle para representar la serpiente y una instancia de Circle para representar la comida
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        // variables booleanas para controlar la dirección de la serpiente
        bool goLeft, goRight, goDown, goUp;


        public Form1()
        {
            // inicializa los componentes del formulario y crea una nueva instancia de la clase Settings para configurar el juego
            InitializeComponent();

            // crea una nueva instancia de la clase Settings para configurar el juego
            new Settings();
        }

        // eventos para manejar las teclas presionadas y soltadas, el botón de inicio, el botón de captura de pantalla, el temporizador del juego y la actualización de los gráficos del PictureBox
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            // actualiza las variables booleanas para controlar la dirección de la serpiente según las teclas presionadas,
            // evitando que la serpiente se mueva en la dirección opuesta a la actual
            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }



        }
        // actualiza las variables booleanas para controlar la dirección de la serpiente según las teclas soltadas
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            // llama al método RestartGame para iniciar el juego
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            // crea una etiqueta para mostrar el puntaje y el récord actual en la imagen capturada, y luego muestra un cuadro de diálogo para guardar la imagen como un archivo JPG
            Label caption = new Label();
            caption.Text = "Tu puntuacion es: " + score + " El mejol saco: " + highScore + " En el juego Alan y Delio";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.Purple;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            // muestra un cuadro de diálogo para guardar la imagen como un archivo JPG
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Segundo_parcial";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;


            // si el usuario selecciona un archivo y hace clic en "Guardar", captura la imagen del PictureBox,
            // la guarda como un archivo JPG y luego elimina la etiqueta de puntaje y récord del PictureBox
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }





        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // actualiza la dirección de la serpiente según las teclas presionadas

            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            // mueve la serpiente en la dirección actual y verifica si ha comido la comida o si ha chocado consigo misma

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {

                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    // verifica si la serpiente ha chocado con los bordes del área de juego y la hace aparecer en el lado opuesto
                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }

                    // verifica si la serpiente ha comido la comida y, si es así, llama al método EatFood para aumentar el puntaje y agregar un nuevo segmento a la serpiente
                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }

                    // verifica si la serpiente ha chocado consigo misma al comparar la posición de la cabeza con la posición de cada segmento del cuerpo,
                    // y si es así, llama al método GameOver para finalizar el juego
                    for (int j = 1; j < Snake.Count; j++)
                    {

                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }

                    }


                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }

            // invalida el área de dibujo del PictureBox para que se actualice la imagen con la nueva posición de la serpiente y la comida
            picCanvas.Invalidate();

        }

        // actualiza los gráficos del PictureBox para dibujar la serpiente y la comida en sus posiciones actuales
        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            // dibuja cada segmento de la serpiente en el PictureBox, usando un color diferente para la cabeza y el cuerpo
            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }

                // dibuja un círculo para cada segmento de la serpiente en el PictureBox usando el color correspondiente
                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }

            // dibuja la comida en el PictureBox usando un color rojo oscuro
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
            food.X * Settings.Width,
            food.Y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        // reinicia el juego configurando el tamaño del área de juego, limpiando la lista de la serpiente, deshabilitando los botones de inicio y captura de pantalla, restableciendo el puntaje,
        // agregando la cabeza de la serpiente a la lista y generando una nueva posición para la comida, y luego inicia el temporizador del juego
        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();

            startButton.Enabled = false;
            snapButton.Enabled = false;
            score = 0;
            txtScore.Text = "Score: " + score;

            Circle head = new Circle { X = 10, Y = 5 };
            // agrega la cabeza de la serpiente a la lista Snake
            Snake.Add(head); 

            for (int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            // genera una nueva posición para la comida usando números aleatorios dentro de los límites del área de juego
            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            // inicia el temporizador del juego para comenzar a actualizar la posición de la serpiente y la comida en el PictureBox
            gameTimer.Start();

        }

        // aumenta el puntaje, actualiza la etiqueta de puntaje, agrega un nuevo segmento a la serpiente y genera una nueva posición para la comida cuando la serpiente come la comida
        private void EatFood()
        {
            score += 1;

            txtScore.Text = "Score: " + score;

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            // agrega un nuevo segmento a la serpiente en la posición del último segmento del cuerpo para que la serpiente crezca cuando come la comida
            Snake.Add(body);

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };


        }

        // finaliza el juego deteniendo el temporizador, habilitando los botones de inicio y captura de pantalla, y actualizando el récord si el puntaje actual es mayor que el récord anterior
        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            snapButton.Enabled = true;

            // actualiza el récord si el puntaje actual es mayor que el récord anterior, y muestra el nuevo récord en la etiqueta de récord con un color y alineación específicos
            if (score > highScore)
            {
                highScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }


    }
}