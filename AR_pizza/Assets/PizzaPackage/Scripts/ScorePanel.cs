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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//класс для отображения финальных результатов
public class ScorePanel : MonoBehaviour
{
    //объявление объектов
    public GameObject[] scoreStars;

    public GameObject finalScorePanel;
    public Text finalScoreText;

    //метод, оценивающий результат раунда
    public void ScoreRound(int score)
    {
        //останавливает все побочные потоки
        StopAllCoroutines();

        Hide();
        //отображает панель очков
        gameObject.SetActive(true);

        //включает звёзды поочередно
        for (int i = 0; i < score; i++)
        {            
            scoreStars[i].SetActive(true);
        }
        //запускает метод HideAfterDelay() в отдельном потоке
        StartCoroutine(HideAfterDelay());
    }

    //метод, который "прячет все звёзды"
    private void Hide()
    {
        //прячет звёзды по одной
        foreach (GameObject star in scoreStars)
        {
            star.SetActive(false);
        }
        //выключает панель с очками
        finalScorePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    //вызывает метод Hide() с задержкой в 1,5 секунды
    private IEnumerator HideAfterDelay()
    {
        //задержка
        yield return new WaitForSeconds(1.5f);
        //вызов метода
        Hide();
    }

    //метод отображает финальный результат
    public void FinalScore(int score)
    {
        //останавливает все побочные потоки
        StopAllCoroutines();
        //выключает звёзды по одной
        foreach(GameObject star in scoreStars)
        {
            star.SetActive(false);
        }
        //включает панель очков
        finalScorePanel.SetActive(true);
        //отображает текст с финальным результатом игрока
        finalScoreText.text = "You scored " + score + " points!";
    }
}
