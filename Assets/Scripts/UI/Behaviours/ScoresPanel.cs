using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class ScoresPanel : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text m_score;
        [SerializeField] private TMPro.TMP_Text m_level;
        [SerializeField] private TMPro.TMP_Text m_lines;

        public void UpdateValues(in CurrentScore score)
        {
            m_score.text = score.score.ToString();
            m_level.text = score.level.ToString();
            m_lines.text = score.lines.ToString();
        }
    }
}
