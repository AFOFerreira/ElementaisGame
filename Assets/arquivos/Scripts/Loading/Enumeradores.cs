public enum LoadingState 
{
    LOGIN,
    MENU_INICIAL,
    TUTORIAL1,
    TUTORIAL2,
    MULTIPLAYER, 
    NONE
}

public enum TipoJogador 
{ 
    PLAYER,
    IA
}
public enum TipoFase
{
    MONSTRO,
    MAGICA,
    DEFESA
}
public enum AudioSounds 
{
    GAMEPLAY,
    MENU,
    LOADING,
    TUTORIAL
}

public enum AudioSFX
{
    BOTAO_A,
    BOTAO_AA,
    BOTAO_B,
    BOTAO_BB,
    BOTAO_C,
    HUD_IN,
    HUD_OUT
}

public enum TipoCarta 
{
    ELEMENTAL,
    MAGICA
}

public enum TipoElemental 
{
    AGUA,
    FOGO,
    TERRA,
    AR
}

public enum TipoMagica 
{
    AUXILIAR,
    ARMAILHA
}

public enum TipoEfeito
{
    UNICO,
    CONTINUO
}

public enum TipoPartida
{
    LOCAL,
    MULTIPLAYER
}