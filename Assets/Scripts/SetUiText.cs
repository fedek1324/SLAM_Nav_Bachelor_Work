using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class SetUiText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textField;
        [SerializeField]
        private string fixedText;

        public void OnSliderValueChanged(float numericValue)
        {
            textField.text = $"{fixedText}: {numericValue}";
        }
    }
}
