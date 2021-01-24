using GPSTCPClient.ViewModel;
using GPSTCPClient.ViewModel.Components;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

namespace GPSTCPClient.Components
{
    public class BindableMap : Map
    {
        public BindableMap() : base()
        {
            this.Unloaded += (o, s) =>
            {
                //this.Children.Clear();
                clearPins();
            };

        }

        #region ULPins
        public ObservableCollection<UserLocation> ULPins
        {
            get { return (ObservableCollection<UserLocation>)GetValue(ULPinsProperty); }
            set { SetValue(ULPinsProperty, value); }
        }

        public static readonly DependencyProperty ULPinsProperty =
        DependencyProperty.Register("ULPins", typeof(ObservableCollection<UserLocation>), typeof(BindableMap),
        new PropertyMetadata(null, new PropertyChangedCallback(OnULPinsChanged)));

        private static void OnULPinsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableMap map = (BindableMap)d;
            map.ClearULMapPoints();
            map.SubscribeToULCollectionChanged();
        }

        private void ClearULMapPoints()
        {
            //this.Children.Clear();
            clearPins();
        }

        private void SubscribeToULCollectionChanged()
        {
            if (ULPins != null)
                ULPins.CollectionChanged += ULPins_CollectionChanged;
        }

        private void ULPins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            Pushpin[] temp = this.Children.Cast<object>().Where(p => p is Pushpin).Cast<Pushpin>().ToArray().Clone() as Pushpin[];

            if (e.OldItems != null)
            {
                //foreach (UserLocation ul in e.OldItems)
                //    this.Children.Remove(temp.FirstOrDefault(p => p.ToolTip == ul.Name));
                clearPins();
                foreach (var pin in temp)
                {
                    if (e.OldItems.Cast<UserLocation>().Any(p => p.Name == (string)((ToolTip)pin?.ToolTip)?.Content)) continue;
                    Pushpin np = new Pushpin() { Location = pin.Location, Content = pin.Content };
                    np.ToolTip = pin.ToolTip;
                    this.Children.Add(np);
                }

            }
                

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (Pushpin pin in temp)
                {
                    if (!Pins.Any(p => p.Location == pin.Location)) this.Children.Remove(pin);
                }
            }

            //Add the new pushpins
            if (e.NewItems != null)
                foreach (UserLocation ul in e.NewItems)
                {
                    Pushpin np = new Pushpin() { Location = ul.Pin.Location, Content = Tools.CreateIcon(MaterialDesignThemes.Wpf.PackIconKind.Star) };
                    np.ToolTip = Tools.CreateTootip(ul.Name);
                    this.Children.Add(np);
                }
            this.UpdateLayout();
        }
        #endregion

        #region Pins
        public ObservableCollection<Pushpin> Pins
        {
            get { return (ObservableCollection<Pushpin>)GetValue(PinsProperty); }
            set { SetValue(PinsProperty, value); }
        }

        public static readonly DependencyProperty PinsProperty =
        DependencyProperty.Register("Pins", typeof(ObservableCollection<Pushpin>), typeof(BindableMap),
        new PropertyMetadata(null, new PropertyChangedCallback(OnPinsChanged)));

        private static void OnPinsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindableMap map = (BindableMap)d;
            map.ClearPinsMapPoints();
            map.SubscribeToPinsCollectionChanged();
        }

        private void ClearPinsMapPoints()
        {
            //this.Children.Clear();
            clearPins();
        }

        private void SubscribeToPinsCollectionChanged()
        {
            if (Pins != null)
                Pins.CollectionChanged += Pins_CollectionChanged;
        }

        private void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Pushpin[] temp = this.Children.Cast<object>().Where(p => p is Pushpin).Cast<Pushpin>().ToArray().Clone() as Pushpin[];
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach(Pushpin pin in temp)
                {
                    if (!ULPins?.Any(p => p.Pin.Location == pin.Location) ?? false) this.Children.Remove(pin);
                }
            }
            //Remove the old pushpins
            if (e.OldItems != null)
                foreach (Pushpin pin in e.OldItems)
                    this.Children.Remove(temp.FirstOrDefault(p => p.Location == pin.Location && p.ToolTip == pin.ToolTip));

            //Add the new pushpins
            if (e.NewItems != null)
                foreach (Pushpin pin in e.NewItems)
                {
                    Pushpin np = new Pushpin() { Location = pin.Location, Content = pin.Content };
                    if (pin.ToolTip != null)
                    {
                        np.ToolTip = Tools.CreateTootip(pin.ToolTip.ToString());
                    }
                    this.Children.Add(np);
                }
                    
        }
        #endregion

        private void clearPins()
        {
            var temp = this.Children.Cast<object>().ToArray().Clone() as object[];
            foreach(var child in temp)
            {
                if(child is Pushpin pp)
                {
                    this.Children.Remove(pp);
                }
            }
        }
    }
}
