using GPSTCPClient.ViewModel.Components;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.ObjectModel;
using System.Windows;

namespace GPSTCPClient.Components
{
    public class BindableMap : Map
    {
        public BindableMap() : base()
        {
            //this.MouseDoubleClick += (o, e) => e.Handled = true;
            this.Children.Clear();
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
            this.Children.Clear();
        }

        private void SubscribeToULCollectionChanged()
        {
            if (ULPins != null)
                ULPins.CollectionChanged += ULPins_CollectionChanged;
        }

        private void ULPins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Remove the old pushpins
            if (e.OldItems != null)
                foreach (UserLocation ul in e.OldItems)
                    this.Children.Remove(ul.Pin);

            //Add the new pushpins
            if (e.NewItems != null)
                foreach (UserLocation ul in e.NewItems)
                    if(ul.Pin != null) this.Children.Add(ul.Pin);
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
            this.Children.Clear();
        }

        private void SubscribeToPinsCollectionChanged()
        {
            if (Pins != null)
                Pins.CollectionChanged += Pins_CollectionChanged;
        }

        private void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Remove the old pushpins
            if (e.OldItems != null)
                foreach (Pushpin pin in e.OldItems)
                    this.Children.Remove(pin);

            //Add the new pushpins
            if (e.NewItems != null)
                foreach (Pushpin pin in e.NewItems)
                    this.Children.Add(pin);
        }
        #endregion
    }
}
