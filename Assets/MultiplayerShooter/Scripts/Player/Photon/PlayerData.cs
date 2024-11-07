using Fusion;
using UnityEngine;
using Zenject;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private Transform[] _skins;
    [SerializeField] private PlayerAnimation _playerAnimation;

    [Networked] public string Nickname { get; set; }

    [Networked] public int HP { get; set; }

    [Networked] public int Kills { get; set; }

    [Networked] public int ActiveSkinIndex { get; set; }

    [Networked] public Weapon ActiveWeapon { get; set; }


    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
        {
            HP = 15;
            Kills = 0;
            int randomIndexCharacter = Random.Range(0, 3);
            ActiveSkinIndex = randomIndexCharacter;
        }
        SetActiveSkin(ActiveSkinIndex);
    }

    public void SetActiveSkin(int skinIndex)
    {
        _playerAnimation.Animator = _skins[ActiveSkinIndex].gameObject.GetComponent<Animator>();
        ApplySkin(skinIndex);
    }

    private void ApplySkin(int skinIndex)
    {
        for (int i = 0; i < _skins.Length; i++)
        {
            _skins[i].gameObject.SetActive(i == skinIndex);
        }
    }
}
