﻿using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CoffeeClientPrototype.Model;
using CoffeeClientPrototype.ViewModel.Services;
using GalaSoft.MvvmLight;

namespace CoffeeClientPrototype.ViewModel.List
{
    public class ListViewModel : ViewModelBase
    {
        private readonly IDataService dataService;

        public ListViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            this.BestCafes = new ObservableCollection<CafeListItem>();
            this.NearbyCafes = new ObservableCollection<CafeListItem>();
        }

        public ObservableCollection<CafeListItem> NearbyCafes { get; private set; }

        public ObservableCollection<CafeListItem> BestCafes { get; private set; }

        public async Task OnNavigatedTo()
        {
            var cafes = await this.dataService.GetAllCafes();
            this.PopulateBestCafes(cafes);
            this.PopulateNearbyCafes(cafes);
        }

        private void PopulateBestCafes(IEnumerable<Cafe> cafes)
        {
            var items = cafes.OrderByDescending(cafe => cafe.Rating)
                .ThenByDescending(cafe => cafe.NumberOfVotes)
                .Take(10)
                .Select(CafeListItem.FromModel);

            foreach (var item in items)
            {
                this.BestCafes.Add(item);
            }
        }

        private void PopulateNearbyCafes(IEnumerable<Cafe> cafes)
        {
            var items = cafes.Select(CafeListItem.FromModel);

            foreach (var item in items)
            {
                this.NearbyCafes.Add(item);
            }
        }
    }
}
