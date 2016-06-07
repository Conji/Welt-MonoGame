﻿#region Copyright
// COPYRIGHT 2016 JUSTIN COX (CONJI)
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Welt.Models;
using Welt.UI.Components;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Welt.UI
{
    public abstract class UIComponent : IDisposable
    {
        public virtual string Name { get; protected set; }
        public virtual int Width { get; set; } = 100;
        public virtual int Height { get; set; } = 100;
        public virtual float Opacity { get; set; } = 1;
        public virtual Cursor Cursor { get; set; } = Cursors.Default;
        public virtual BoundsBox Margin { get; set; } = new BoundsBox();
        public virtual BoundsBox Padding { get; set; } = new BoundsBox();
        public virtual HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public virtual VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
        public virtual bool IsActive { get; set; } = true;

        public bool IsMouseOver { get; private set; }
        public bool IsLeftMouseDown { get; private set; }
        public bool IsRightMouseDown { get; private set; }

        public virtual int X { get; set; }
        public virtual int Y { get; set; }

        internal virtual UIComponent Parent { get; set; }
        // TODO: consider making this static then have each component indexed so we can tell when the batch should push
        protected SpriteBatch Sprite { get; set; }
        protected Rectangle Region => new Rectangle(X, Y, Width, Height);
       
        protected virtual Texture2D Texture { get; set; }
        protected virtual GraphicsDevice Graphics { get; private set; }
        
        protected virtual Dictionary<string, UIComponent> Components { get; set; }
        internal bool IsSizeProcessed;

        protected static string CurrentlySelectedTextComponent;
        private static Cursor _currentCursor = Cursor.Current;

        protected UIComponent(string name, int width, int height, GraphicsDevice device) : this(name, width, height, null, device)
        {
            
        }

        protected UIComponent(string name, int width, int height, UIComponent parent, GraphicsDevice device)
        {
            Name = name;
            if (parent != null)
            {
                Width = width == -1 ? parent.Width : width;
                Height = height == -1 ? parent.Height : height;
            }
            else
            {
                Width = width == -1 ? WeltGame.Width : width;
                Height = height == -1 ? WeltGame.Height : height;
            }
            
            Components = new Dictionary<string, UIComponent>(16); // default size is 8 children.
            Parent = parent;
            Graphics = device;
            Sprite = new SpriteBatch(Graphics);
            
        }

        internal void ProcessArea()
        {
            if (IsSizeProcessed) return;
            IsSizeProcessed = true;

            // Know what, lets redo this. I feel like there's somethings that aren't working out with it.
            #region Old Implementation
            //float x = X;
            //float y = Y;
            //float width;
            //float height;

            //switch (HorizontalAlignment)
            //{
            //    case HorizontalAlignment.Right:
            //        width = Parent?.Width ?? WeltGame.Width;
            //        x = X + (width - Width) - Margin.Right;
            //        break;
            //    case HorizontalAlignment.Center:
            //        width = Parent?.Width ?? WeltGame.Width;
            //        x = X + (width - Width)/2 + Margin.Left - Margin.Right;
            //        break;
            //    case HorizontalAlignment.Left:
            //        x = X + Margin.Left;
            //        break;
            //}

            //switch (VerticalAlignment)
            //{
            //    case VerticalAlignment.Bottom:
            //        height = Parent?.Height ?? WeltGame.Height;
            //        y = Y + (height - Height) - Margin.Bottom;
            //        break;
            //    case VerticalAlignment.Center:
            //        height = Parent?.Height ?? WeltGame.Height;
            //        y = Y + (height - Height)/2 + Margin.Top - Margin.Bottom;
            //        break;
            //    case VerticalAlignment.Top:
            //        y = Y + Margin.Top;
            //        break;
            //}

            //X = (int) x;
            //Y = (int) y;
            #endregion  
            // first lets create a local X and Y object that we can adjust for what-not.
            var x = X;
            var y = Y;
            if (Parent != null)
            {
                if (x < Parent.X) X = Parent.X; // if it's too far to the left
                if (x > Parent.X) X = Parent.X - Width; // if it's too far to the right
                if (y < Parent.Y) y = Parent.Y; // if it's too far up
                if (y > Parent.Y) y = Parent.Y - Height; // if it's too far down
            }
            // now that we have our new X and Y of the component, lets adjust X and Y based on alignments
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    // since it's to the left, we need to do nothing except margin and padding adjustment.
                    x += (int) Margin.Left;
                    if (Parent != null) x += (int) Parent.Padding.Left;
                    break;
                case HorizontalAlignment.Center:
                    // to figure the center area, first we get the width of the next highest component then subtract the left
                    // margin from it (this is the available area we can put a component in).
                    var ownerWidth = (Parent?.Width ?? WeltGame.Width) - (int) Margin.Left;
                    // next we get the width difference and divide by two so we know how much space must be on
                    // the left and the right
                    var centerSpace = (ownerWidth - Width)/2;
                    // afterwards we add that to the X position (and cross our fingers it works)
                    x += centerSpace;

                    break;
                case HorizontalAlignment.Right:
                    // it's a little easier than center when going to the right. The X does not matter before hand.
                    // first we subtract the width from the parent's X + width
                    x = (Parent?.X ?? 0 + Parent?.Width ?? WeltGame.Width) - Width;
                    // next we check the margin and padding of the parent
                    if (Parent != null) x -= (int) Parent.Padding.Right;
                    x -= (int) Margin.Right;
                    break;
            }
            X = x;

            // next we work on vertical alignment. It's pretty much the same as horizontal alignment.
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    y += (int) Margin.Top;
                    if (Parent != null) y += (int) Parent.Padding.Top;
                    break;
                case VerticalAlignment.Center:
                    var ownerHeight = (Parent?.Height ?? WeltGame.Height) - (int) Margin.Top;
                    var centerSpace = (ownerHeight - Height)/2;
                    y += centerSpace;
                    break;
                case VerticalAlignment.Bottom:
                    y = (Parent?.Y ?? 0 + Parent?.Height ?? WeltGame.Height) - Height;
                    // next we check the margin and padding of the parent
                    if (Parent != null) y -= (int) Parent.Padding.Bottom;
                    y -= (int) Margin.Bottom;
                    break;
            }
        }
        
        protected bool GetMouseOver(out MouseState mouse)
        {
            mouse = Mouse.GetState();
            return Region.Contains(mouse.X*WeltGame.WidthViewRatio, mouse.Y*WeltGame.HeightViewRatio);
        }

        #region Public Methods

        public virtual void Initialize()
        {
            ProcessArea();
            // TODO: creation logic here. Mainly spritebatch creation logic.
            foreach (var child in Components.Values)
            {
                child.Parent = this;
                child.Initialize();
            }
        }

        public virtual void Update(GameTime time)
        {
            MouseState mouse;
            foreach (var child in Components.Values)
            {
                child.Update(time);
            }

            if (GetMouseOver(out mouse))
            {
                // TODO: ARGS for the mouse
                if (!IsMouseOver)
                {
                    Parent?.MouseEnter?.Invoke(this,
                        new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                    MouseEnter?.Invoke(this,
                        new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                    IsMouseOver = true;
                    // change the cursor
                    WeltGame.SetCursor(Cursor);
                }
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!IsLeftMouseDown)
                    {
                        Parent?.MouseLeftDown?.Invoke(this,
                            new MouseEventArgs(MouseButtons.Left, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        MouseLeftDown?.Invoke(this,
                            new MouseEventArgs(MouseButtons.Left, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        IsLeftMouseDown = true;
                        CurrentlySelectedTextComponent = Name;
                        GainFocus?.Invoke(this, null);
                    }
                }
                else
                {
                    if (IsLeftMouseDown)
                    {
                        Parent?.MouseLeftUp?.Invoke(this,
                            new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        MouseLeftUp?.Invoke(this,
                            new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        IsLeftMouseDown = false;
                        LostFocus?.Invoke(this, null);
                    }
                }

                if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (!IsRightMouseDown)
                    {
                        Parent?.MouseRightDown?.Invoke(this,
                            new MouseEventArgs(MouseButtons.Right, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        MouseRightDown?.Invoke(this,
                            new MouseEventArgs(MouseButtons.Right, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        IsRightMouseDown = true;
                        CurrentlySelectedTextComponent = Name;
                    }
                }
                else
                {
                    if (IsRightMouseDown)
                    {
                        Parent?.MouseRightUp?.Invoke(this,
                            new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        MouseRightUp?.Invoke(this,
                            new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                        IsRightMouseDown = false;
                    }
                }
                if (Cursor.Current != Cursor) Cursor.Current = Cursor;
            }
            else
            {
                if (IsMouseOver)
                {
                    Parent?.MouseLeave?.Invoke(this,
                        new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                    MouseLeave?.Invoke(this,
                        new MouseEventArgs(MouseButtons.None, 0, mouse.X, mouse.Y, mouse.ScrollWheelValue));
                    IsMouseOver = false;
                    ((Form) Control.FromHandle(WeltGame.Instance.Window.Handle)).ResetCursor();
                }
                IsRightMouseDown = false;
                IsLeftMouseDown = false;
            }

            foreach (var child in Components.Values)
            {
                child.Update(time);
            }

            (this as PasswordBoxComponent)?.ToggleSelected(CurrentlySelectedTextComponent == Name);
            (this as TextInputComponent)?.ToggleSelected(CurrentlySelectedTextComponent == Name);
        }


        public virtual void Draw(GameTime time)
        {
            foreach (var child in Components.Values.Where(child => child.IsActive))
            {
                child.Draw(time);
            }
        }

        public virtual void AddComponent(UIComponent component)
        {
            Components.Add(component.Name, component);
        }

        public virtual void RemoveComponent(string name)
        {
            Components.Remove(name);
        }

        public virtual Maybe<UIComponent, NullReferenceException> GetComponent(string name)
        {
            foreach (var child in Components)
            {
                if (child.Key == name) return new Maybe<UIComponent, NullReferenceException>(child.Value, null);
                var m = Maybe<UIComponent, NullReferenceException>.Check(() => child.Value.GetComponent(name).Value);
                if (m.HasError) continue;
                return new Maybe<UIComponent, NullReferenceException>(child.Value, null);
            }
            return new Maybe<UIComponent, NullReferenceException>(null, new NullReferenceException());
        }

        #endregion

        #region Private Methods
        


        #endregion

        public event EventHandler<MouseEventArgs> MouseEnter;
        public event EventHandler<MouseEventArgs> MouseLeave;
        public event EventHandler<MouseEventArgs> MouseLeftDown;
        public event EventHandler<MouseEventArgs> MouseLeftUp;
        public event EventHandler<MouseEventArgs> MouseRightDown;
        public event EventHandler<MouseEventArgs> MouseRightUp;
        public event EventHandler GainFocus;
        public event EventHandler LostFocus;

        public static UIProperty<float> OpacityProperty = new UIProperty<float>("opacity");
        public static UIProperty<int> WidthProperty = new UIProperty<int>("width");
        public static UIProperty<int> HeightProperty = new UIProperty<int>("height");
        public static UIProperty<BoundsBox> MarginProperty = new UIProperty<BoundsBox>("margin");
        public static UIProperty<BoundsBox> PaddingProperty = new UIProperty<BoundsBox>("padding");
        public static UIProperty<HorizontalAlignment> HorizontalAlignmentProperty = new UIProperty<HorizontalAlignment>("horizontalalignment");
        public static UIProperty<VerticalAlignment> VerticalAlignmentProperty = new UIProperty<VerticalAlignment>("verticalalignment");
        public static UIProperty<bool> IsActiveProperty = new UIProperty<bool>("isactive");

        public Maybe<PropertyInfo, NullReferenceException> GetPropertyValue<T>(UIProperty<T> property)
        {
            var m = Maybe<PropertyInfo, NullReferenceException>.Check(() =>
            {
                return GetType().GetProperties().First(p => p.Name.ToLower() == property.Name.ToLower());
            });
            return m;
        }

        public void SetPropertyValue<T>(UIProperty<T> property, T value)
        {
            var m = Maybe<PropertyInfo, NullReferenceException>.Check(() =>
            {
                return GetType().GetProperties().First(p => p.Name.ToLower() == property.Name.ToLower());
            });

            if (m.HasError) Console.WriteLine(m.Error);
            else m.Value.SetValue(this, value, BindingFlags.IgnoreCase, null, null, null);
            ProcessArea();
        }

        public static UIComponent Clone(UIComponent comp, string name)
        {
            var n = comp;
            n.Name = name;
            return n;
        }
        
        public virtual void Dispose()
        {
            MouseEnter = null;
            MouseLeave = null;
            MouseLeftDown = null;
            MouseLeftUp = null;
            MouseRightDown = null;
            MouseRightUp = null;
            foreach (var child in Components)
            {
                child.Value.Dispose();
            }
            Components.Clear();
            Parent = null;
            Graphics = null;
        }
    }
}