using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Noble/Create new Noble")]
public class Noble : ScriptableObject
{
    [SerializeField] private string nobleName;
    [SerializeField] private NobleHouse house;
    [SerializeField] private Noble mother;
    [SerializeField] private Noble father;
    [SerializeField] private int age;
    [SerializeField] private bool male = true; //whether or not the character is male. Important for familymaking.

    public string NobleName { get { return nobleName; } }
    public NobleHouse House { get { return house; } }
    public Noble Mother { get { return mother; } }
    public Noble Father { get { return father; } }
    public int Age { get { return age; } }
    public bool Male { get { return male; } }

    public Noble(string nobleName, Noble mother, Noble father, NobleHouse house, int age, bool male)
    {
        this.nobleName = nobleName;
        this.mother = mother;
        this.father = father;
        this.house = house;
        this.age = age;
        this.male = male;

        this.house.addNoble(this); //finally, add to the house.
    }
}

[CreateAssetMenu(menuName = "Noble/Create new Noble House")]
public class NobleHouse : ScriptableObject
{
    [SerializeField] private string houseName;
    private Region homeland; //the home region for this house.
    [SerializeField] private List<Noble> characters;
    [SerializeField] private Noble headOfHouse;
    public List<Noble> Characters { get { return characters; } }
    public Noble HeadOfHouse { get { return headOfHouse; } }
    public string HouseName { get { return houseName; } }
    public Region Homeland { get { return homeland; } }

    public void addNoble(Noble noble)
    {
        characters.Add(noble);
    }
}