/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
//импорт библиотек
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

/*структура заказа, т.е эллементы, которые входят в нашу пиццу.
 переменные имеют булевый тип*/
public struct Order
{
    public bool pepperoni;
    public bool pineapple;
    public bool mushroom;
}

public class GameManager : MonoBehaviour
{
    //игровые объекты, которые будут появляться на сцене
    public GameObject pizza;
    public GameObject pepperoni;
    public GameObject mushroom;
    public GameObject pineapple;
    
    //объект текст для работы с таймером
    public Text timer;

    //экзмепляр данного скрипта
    public static GameManager instance;

    //оставшееся время
    public static float timeRemaining;
    //очки раунда
    private static int roundScore;
    //объект класса Dictionary, хранит в себе количество 1,2,3,4 и 5-звёздочных результатов 
    private static Dictionary<int, int> accumulatedScore = new Dictionary<int, int>(5);

    //переменная, хранящяя состояние игры(окончена/не окончена)
    bool gameIsOver = false;

    //заказ
    public Order currentOrder;
    //события, выполняемые при определенных условиях
    public UnityEvent onNewOrderCreated;
    public IntEvent onOrderComplete;

    public IntEvent onGameComplete;

    public UnityEvent onGameRestart;
    
    // метод, который вызывается единожды, при инициализации
    private void Start()
    {
        //метод запускает игру если она не запущена
        if (instance == null)
        {
            instance = this;
            RestartGame();
        }
        else
        {
            //удаление текущего игрового объекта
            Destroy(this);
        }
    }


    //метод обнуляет время и игровые очки. т.е производит первичную инициализацию
    private void Init()
    {
        roundScore = 3;//очки раунда
        timeRemaining = 60;//оставшееся время
        accumulatedScore.Clear();  //очищает предыдущие результаты
        accumulatedScore.Add(1, 0); //1 звезда
        accumulatedScore.Add(2, 0); //2 звезды
        accumulatedScore.Add(3, 0); //3 звезды
        accumulatedScore.Add(4, 0); //4 звезды
        accumulatedScore.Add(5, 0); //5 звезд
        gameIsOver = false;//игра не окончена
        CreateNewOrder();
    }

    //метод проверяет результат игрока на соответствие заказу
    public void CompleteOrder()
    {
        //оценка сделанной пиццы
        roundScore = 2;
        //если наличие ингридиента в заказе и в пицце совпадает, к счёту добавляется очко, если нет - отнимается
        roundScore = mushroom.activeInHierarchy == currentOrder.mushroom ?  roundScore + 1 : roundScore - 1;
        roundScore = pepperoni.activeInHierarchy == currentOrder.pepperoni ?  roundScore + 1 : roundScore - 1;
        roundScore = pineapple.activeInHierarchy == currentOrder.pineapple ?  roundScore + 1 : roundScore - 1;

        //перевод очков 5-звёздночную систему
        roundScore = Mathf.Clamp(roundScore, 1, 5);
        //вызывает событие, отвечающее за обработку выполненого заказа
        onOrderComplete.Invoke(roundScore);
        Debug.Log(roundScore);
        //добавление пиццы в общий счёт
        Debug.Log(accumulatedScore.Keys.Count + " " + accumulatedScore.Values.Count);
        accumulatedScore[roundScore]++;

        //проверка на то, осталось ли время, чтобы сделать ещё пиццу(осталось ли ещё время)
        if (timeRemaining > 0)
        {       
            //создаёт новый заказ
            CreateNewOrder();
        }
        else
        {
            //конец игры
            GameComplete();
        }
    }

    //метод, создающий новый заказ
    public void CreateNewOrder()
    {
        //обнуление предыдущих параметров заказа
        mushroom.SetActive(false);
        pineapple.SetActive(false);
        pepperoni.SetActive(false);

        //создание нового заказа со случайными параметрами
        currentOrder = new Order();
        currentOrder.mushroom = Random.value > 0.5f;
        currentOrder.pepperoni = Random.value > 0.5f;
        currentOrder.pineapple = Random.value > 0.5f;

        //вызывает событие, отвечающее за обработку созданного заказа
        onNewOrderCreated.Invoke();
    }


    //метод, в отличии от Start() вызывается каждый кадр
    public void Update()
    {
        //проверка на оставшееся время
        if (timeRemaining > 0)
        {
            //отнимает время
            timeRemaining -= Time.deltaTime;
            //отображает оставшееся время на таймере
            if(timer != null)
            {
                //отображает текст
                timer.text = Mathf.RoundToInt(timeRemaining).ToString();
            }
            
        }
        else if (!gameIsOver)//завершает текущий заказ и игру по истечению времени на таймере
        {
            gameIsOver = true;
            CompleteOrder();
        }
    }
  

    //метод, вызываемый при окончании игры
    private void GameComplete()
    {
        //подсчёт финального результата игрока(счёт)
        int totalScore = 0;
        //5 звезд прибавляют к счёту 5 очков, 4 - 4, и т.д
        totalScore += accumulatedScore[5] * 5;
        totalScore += accumulatedScore[4] * 4;
        totalScore += accumulatedScore[3] * 3;
        totalScore += accumulatedScore[2] * 2;
        totalScore += accumulatedScore[1] * 1;

        //вызывает событие, отвечающее за обработку окончания игры ScorePanel.FinalScore()
        onGameComplete.Invoke(totalScore);
    }

    //метод, перезапускающий игру
    public void RestartGame()
    {
        //вызывает событие, отвечающее за обработку перезапуска игры ScorePanel.FinalScore()
        onGameRestart.Invoke();
        Init();
    }
}
