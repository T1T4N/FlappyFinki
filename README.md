FlappyFinki
===========

### 1. Опис на апликацијата
Идејата за проектната задача е земена од популарната игра “Flappy Bird”. Всушност и самата таа претставува игра многу слична на “Flappy Bird”, мегутоа направена во ФИНКИ стил и затоа се вика “Flappy Finki” .

Во играта ние сме едно мало суштество (играч) наречено “FinkiMan” коешто ја има формата на логото на ФИНКИ. Со помош на скокови треба да поминуваме низ препреките (кои претставуваат цефки) кои имаат отвори на некое рандом место (секој отвор е со иста големина одредена експериментално да не биде ни многу тесна, па да биде претешка играта но да не биде ни многу широка па играта да стане досадна). Бојата на секоја наредна препрека е различна од претходната со цел играта да деува поживо (и тие се одредуваат на рандом). Со секое успешно поминување на препрека добиваме еден поен (под успешно поминување на препрека се мисли целосно да се помине препреката без играчот да допре со било кој дел, било каде на препреката). Играта завршува кога ке го удриме играчот во препрека и тогаш тоа умира (визуелно паѓа на дното на екранот).

### 2. Упатство за користење
+ Пред почетокот на играта внесуваме име под кое сакаме да играме.
+ Со помош на левиот клик на глувчето или на копчето “space” (празно место) го придвижуваме играчот т.е. тоа скока. 
+ Со притискање на копчето “P” (pause) играта се паузира, односно замрзнува додека повторно не го притиснеме истото копче.

      ![paused](https://github.com/T1T4N/FlappyFinki/blob/master/FlappyFinki/images/paused.png?raw=true "Paused screen")

+ Со притискање на копчето “Q” (quit) се излегува од играта, односно прво ни се прикажува МessageBox за да го потврдиме излгувањето во случај по грешка да сме го притиснале копчето.

      ![quit](https://github.com/T1T4N/FlappyFinki/blob/master/FlappyFinki/images/quit.png?raw=true "Quit question")

+ По завршување на играта има вградено најдобри резултати (High Scores) и нашиот го внесува и ни ги прикажува.

      ![highscores](https://github.com/T1T4N/FlappyFinki/blob/master/FlappyFinki/images/highscore.png?raw=true "Highscores")

### 3. Опис на решението на проблемот
#### 3.1 Класи и структури
+	**GameForm** – главна класа за визуелен приказ на играта (исцртување на играчот и препреките и приказ на резултатот), за проверка дали играта е завршена, за справување со влезовите од тастатура и глувче.
+	**Wall** – класа за репрезентација на препреките (цефките), ја содржи колекцијата на бои за ѕидовите, методи и податоци за исцртување на ѕидовите. Метода која проверува дали се случил судир (Intersect), ја користи Graphics класата за исцртување на препреките.
+	**Game** – главна класа која чува податоци за играчот (име и резултат), локација на играчот (на екран), податоци за ѕидовите, метода за исцртување на играчот (PaintPlayer), метода за поместување на ѕидовите (PaintObstacles), исто така контролира кога играчот добива поен. При придвижувањето на играчот е имплементирано забрзување при паѓање и агол (falling speed и rotation), и ротација на играчот постигната со матрица на трансформација. Играчот се исцртува во 5 посебни чекори.
+	**Stats** – помошна класа за чување на името на играчот, резултатот и датата кога е постигнат резултатот со цел да се прикажат во формата најдобри резултати. 
+	**FrmHighscores** – форма едноставно имплементирана со ListBox за прикажувње на најдобрите резултати, соритани според поени. 
+	**FrmMain** – почетна форма со опции за почеток на нова игра или преглед на најдобрите резултати, и исто така ги чува последните изиграни резултати во SortedSet.

#### 3.2 Опис на класата Game
Секој објект (играчот и препреките) претставува објект од класата Rectangle т.е. локација кајшто треба да бидат исцртани елементите. При иницијализирање на класата се задаваат името на играчот, големината на прозорецот за игра и координатите заедно со големината на играчот. 
```c#
public Game(string PlayerName, Point MaxSize, Rectangle instance)
{
	Score = new Stats(PlayerName, 0);

	maxSize = new Point(MaxSize.Y - 38, MaxSize.X - 5);
	Instance = instance;
	Active = true;
	graphics = new Bitmap(Instance.Width + 1, Instance.Height + 1);
	p = Graphics.FromImage(graphics);

	Rectangle topBox = new Rectangle(new Point(maxSize.X - 64, -1), new Size(64, r.Next(90, 200)));
	Rectangle lowerBox = new Rectangle(new Point(maxSize.X - 64, topBox.Height + 235),
		new Size(64, maxSize.Y - topBox.Height + 300));
	wall1 = new Wall(topBox, lowerBox);

	Rectangle topBox2 = new Rectangle(new Point(maxSize.X + (maxSize.X/2) - 64, -1),
		new Size(64, r.Next(90, 200)));
	Rectangle lowerBox2 = new Rectangle(new Point(maxSize.X + (maxSize.X/2) - 64, topBox2.Height + 235),
		new Size(64, maxSize.Y - topBox2.Height + 300));
	wall2 = new Wall(topBox2, lowerBox2);
}
```

Од овие податоци се изведени локацијата и големината на горниот и долниот дел на двете препреки. Принциот на играта е што препреките константно се придвижуваат од десно на лево со тоа што во моментот кога најлевата препрека ке исчезне се појавува од десната страна и ни дава илузија како да е нова препрека, во суштина двете препреки постојано ротираат.

За играчот постои зададена константна брзина на паѓање и според неа се одредува y-локацијата (вертикалната локација). Овде е имплементирано и придвижувањето на играчот (скокот) односно при клик на копче се задава ротација, се намалува брзината и се поместува локацијата на играчот (координатите). Во зависност од тоа дали има скок или не се креира матрицата на трансформација со негативен или позитивен агол (ако има скок аголот е позитивен во спротивно е негативен). 
```c#
int offset = 10;

Rectangle stub = new Rectangle(Instance.X, Instance.Y, offset, Instance.Height);
Rectangle circle = new Rectangle(Instance.X + offset + 3, Instance.Y, Instance.Width - (2*offset),
	Instance.Height);
Rectangle innerCircle = new Rectangle(circle.X + (offset + 1)/2, circle.Y + (offset + 1)/2,
	circle.Width - offset, circle.Height - offset);
Rectangle eye = new Rectangle(circle.Right - 7, circle.Y + 3, 15, 15);
Rectangle zen = new Rectangle(eye.X + 7, eye.Y + 5, 6, 6);


p.FillRectangle(new SolidBrush(Color.Blue), stub);

p.FillEllipse(new SolidBrush(Color.SkyBlue), circle);
p.FillEllipse(new SolidBrush(Color.White), innerCircle);
p.DrawEllipse(Pens.Black, circle);
p.DrawEllipse(Pens.Black, innerCircle);


p.FillEllipse(new SolidBrush(Color.White), eye);
p.FillEllipse(new SolidBrush(Color.Blue), zen);
p.DrawEllipse(Pens.Black, eye);
p.DrawEllipse(Pens.Black, zen);
```
Формата на играчот е составена од 4 различни кругови и еден правоаголник. Референтна точка е правоаголникот и големината на квадратот на играчот. Fill* функциите служат за пополнување на формата на играчот со зададената боја, а draw функциите се користат за исцртување на контурите.

Поен на играчот се задава во моментот кога неговата Х- координата ке ја помине десната координата на првата или вторат а препрека (целосно ке ја помине препреката).
```c#
if (Location.X > wall1.topWall.X + wall1.topWall.Width && !scoreGiven)
{
	Score.IncreaseScore();
	scoreGiven = true;
}
if (Location.X > wall2.topWall.X + wall2.topWall.Width && !scoreGiven)
{
	Score.IncreaseScore();
	scoreGiven = true;
}
```

За крај да ја објасниме функцијата за судир. Прво проверуваме дали играчот се наога во рамки на прозорецот (односно да не се удри во таванот или дното од прозорецот), потоа проверуваме дали има пресечна точка на квадратот на играчот (контурите) со било која координата на било која од двете препреки (ова го вклучува и телото а и отворот на препреките) и доколу има ја завршува играта, се блокира влезор од глувче или тастарута и играчот паѓа.
```c#
public bool CheckCollision()
{
	if ((OldLocation.Y + Instance.Height) >= maxSize.Y || OldLocation.Y <= 0)
	{
		Active = false;
		fallingRotation = 0;
		return true;
	}

	//Check for wall collision.
	Rectangle collide = new Rectangle(OldLocation.X, OldLocation.Y, Instance.Width, Instance.Height);
	if (wall1.Intersect(collide) || wall2.Intersect(collide))
	{
		Active = false;
		fallingRotation = 0;
		return true;
	}
	return false;
}
```
### Изработиле
+ Арменски Роберт
+ Фурнаџиски Никола

##### Се надеваме дека играта ќе ви се допадне и пријатна игра :)
