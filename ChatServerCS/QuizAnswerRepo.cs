using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerCS
{
    public class QuizAnswerRepo
    {
        private List<string> _AnswerList;

        public List<string> AnswerList { get => _AnswerList; set => _AnswerList = value; }
        
        private string _CurrentAnswer;
        public string CurrentAnswer { get => _CurrentAnswer; set => _CurrentAnswer = value; }


        public QuizAnswerRepo()
        {
            AnswerList = new List<string>();
            AnswerList.Add("흡연");
            AnswerList.Add("물티슈");
            AnswerList.Add("빨래바구니");
            AnswerList.Add("설거지");
            AnswerList.Add("라이터");
            AnswerList.Add("로션");
            AnswerList.Add("에어컨");
            AnswerList.Add("밥솥");
            AnswerList.Add("스마트폰");
            AnswerList.Add("이지케어텍");
            AnswerList.Add("김기윤");
            AnswerList.Add("열정");
            AnswerList.Add("창의");
            AnswerList.Add("전문성");
            AnswerList.Add("엣지");
            AnswerList.Add("넥스트");
            AnswerList.Add("신뢰");
        }

        public string GenerateAnwer()
        {
            if (!string.IsNullOrEmpty(CurrentAnswer)) AnswerList.Remove(CurrentAnswer);
            Random rm = new Random();
            CurrentAnswer = AnswerList[rm.Next(0, AnswerList.Count)];
            return CurrentAnswer;
        }


    }
}
