// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    /// <summary>
    /// This is the base class for all the popup field elements.
    /// TValue and TChoice can be different, see MaskField,
    ///   or the same, see PopupField
    /// </summary>
    /// <typeparam name="TValueType"> Used for the BaseField</typeparam>
    /// <typeparam name="TValueChoice"> Used for the choices list</typeparam>
    public abstract class BasePopupField<TValueType, TValueChoice> : BaseField<TValueType>
    {
        internal List<TValueChoice> m_Choices;
        TextElement m_TextElement;
        protected TextElement textElement
        {
            get { return m_TextElement; }
        }

        internal Func<TValueChoice, string> m_FormatSelectedValueCallback;
        internal Func<TValueChoice, string> m_FormatListItemCallback;

        // This is the value to display to the user
        internal abstract string GetValueToDisplay();

        internal abstract string GetListItemToDisplay(TValueType item);

        // This method is used when the menu is built to fill up all the choices.
        internal abstract void AddMenuItems(GenericMenu menu);

        internal virtual List<TValueChoice> choices
        {
            get { return m_Choices; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                m_Choices = value;
            }
        }

        public override void SetValueWithoutNotify(TValueType newValue)
        {
            base.SetValueWithoutNotify(newValue);
            m_TextElement.text = GetValueToDisplay();
        }

        public string text
        {
            get { return m_TextElement.text; }
        }

        public new static readonly string ussClassName = "unity-base-popup-field";
        public static readonly string textUssClassName = ussClassName + "__text";

        internal BasePopupField()
            : this(null) {}

        internal BasePopupField(string label)
            : base(label, null)
        {
            AddToClassList(ussClassName);

            m_TextElement = new TextElement
            {
                pickingMode = PickingMode.Ignore
            };
            m_TextElement.AddToClassList(textUssClassName);
            visualInput.Add(m_TextElement);

            choices = new List<TValueChoice>();
        }

        protected internal override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (evt == null)
            {
                return;
            }

            if (((evt as MouseDownEvent)?.button == (int)MouseButton.LeftMouse) ||
                ((evt.eventTypeId == KeyDownEvent.TypeId()) && ((evt as KeyDownEvent)?.character == '\n') || ((evt as KeyDownEvent)?.character == ' ')))
            {
                ShowMenu();
                evt.StopPropagation();
            }
        }

        private void ShowMenu()
        {
            var menu = new GenericMenu();
            AddMenuItems(menu);
            menu.DropDown(visualInput.worldBound);
        }
    }
}