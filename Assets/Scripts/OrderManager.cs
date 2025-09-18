using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro for UI
public class OrderManager : MonoBehaviour
{
    // Struct to hold UI components for each Item
    [System.Serializable]
    public struct ItemData
    {
        public TMP_Dropdown itemDropdown;   // TMP_Dropdown for selecting the item
        public TextMeshProUGUI unitPriceText; // TextMeshProUGUI to display the unit price
        public TMP_Dropdown qtyDropdown;    // TMP_Dropdown for selecting quantity
    }

    // Array of items (currently one item, can be expanded)
    public ItemData[] items;

    // TextMeshProUGUI for total price
    public TextMeshProUGUI totalPriceText;

    // Dictionary to store item prices
    private Dictionary<string, float> itemPrices;
    private List<string> itemOptions;

    void Start()
    {
        // Initialize item options and prices
        itemOptions = new List<string> { "A4 Folder", "A5 Notebook", "Tissue Box", "Sticky Note Pack", "Ballpoint Pens" };
        itemPrices = new Dictionary<string, float>
        {
            { "A4 Folder", 1.25f },
            { "A5 Notebook", 5.75f },
            { "Tissue Box", 1.50f },
            { "Sticky Note Pack", 2.00f },
            { "Ballpoint Pens", 1.00f }
        };

        // Initialize each item
        for (int i = 0; i < items.Length; i++)
        {
            // Populate item dropdown
            PopulateDropdown(items[i].itemDropdown, itemOptions);

            // Populate quantity dropdown (1-10)
            List<string> qtyOptions = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            PopulateDropdown(items[i].qtyDropdown, qtyOptions);

            // Set initial unit price
            UpdateUnitPrice(items[i]);

            // Add listeners to update total price on change
            int index = i; // Capture index for the listener
            items[i].itemDropdown.onValueChanged.AddListener(delegate { UpdateUnitPrice(items[index]); CalculateTotalPrice(); });
            items[i].qtyDropdown.onValueChanged.AddListener(delegate { CalculateTotalPrice(); });
        }

        // Calculate initial total price
        CalculateTotalPrice();
    }

    // Populate a TMP_Dropdown with options
    private void PopulateDropdown(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    // Update unit price text based on selected item
    private void UpdateUnitPrice(ItemData item)
    {
        string selectedItem = item.itemDropdown.options[item.itemDropdown.value].text;
        float price = itemPrices[selectedItem];
        item.unitPriceText.text = $"${price:F2}";
    }

    // Calculate and update total price
    private void CalculateTotalPrice()
    {
        float total = 0f;

        // Calculate total for all items
        foreach (var item in items)
        {
            string selectedItem = item.itemDropdown.options[item.itemDropdown.value].text;
            int qty = int.Parse(item.qtyDropdown.options[item.qtyDropdown.value].text);
            total += itemPrices[selectedItem] * qty;
        }

        // Update total price text
        totalPriceText.text = $"Total Price: ${total:F2}";
    }
}