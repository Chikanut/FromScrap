using DG.Tweening;
using Packages.Common.Storage.Config;
using UI.Screens.Loading;
using UnityEngine;

public class EndGameTestUILogic : MonoBehaviour
{

    [SerializeField] private EndGameScreenView _endGameScreen;
    
    [SerializeField] private int currentExperience;
    [SerializeField] private int experienceGained;
    [SerializeField] private int currentLevel;

    [SerializeField] private Vector2 _killsStats;
    [SerializeField] private Vector2 _damageStats;
    [SerializeField] private Vector2 _levelStats;
    [SerializeField] private Vector2 _timeStats;

    [SerializeField] private Vector2 _scrapInfo;
    
    [SerializeField] private PlayerProgressionConfigScriptable _playerProgression;

    [SerializeField] private bool _update;

    Sequence _levelSequence;
    private Vector2 _startExperiencePosition;

    public void Update()
    {
        if (_update)
        {
            _endGameScreen.UpdateXP(currentLevel, currentExperience, experienceGained,
                _playerProgression.PlayerLevels.LevelsExperience);
            
            _endGameScreen.UpdateStats((int)_killsStats.x,(int)_killsStats.y,
                (int)_damageStats.x,(int)_damageStats.y,
                (int)_levelStats.x,(int)_levelStats.y,
                _timeStats.x,(int)_timeStats.y);

            _endGameScreen.UpdateScrap((int) _scrapInfo.x, (int) _scrapInfo.y);
            
            _update = false;
        }
    }

   
}
