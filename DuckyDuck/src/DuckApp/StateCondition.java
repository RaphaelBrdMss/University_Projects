package DuckApp;

public class StateCondition {
    public String m_name;
    public EStateCondition m_condition;
    public boolean m_activated = false;


    public StateCondition()
    {
        m_name = "X";
        m_condition = EStateCondition.NONE;
        m_activated = (m_condition == EStateCondition.NONE);
    }

    public StateCondition(EStateCondition e_state)
    {
        m_condition = e_state;
        m_name = e_state.name();
        m_activated = (m_condition == EStateCondition.NONE);
    }

    public StateCondition(EStateCondition e_state, boolean active)
    {
        m_condition = e_state;
        m_name = e_state.name();
        m_activated = active;
    }

    public void activate() { m_activated = true; }
    public void deactivate() { m_activated = false; }
    public void setActivation(boolean active) { m_activated = active;}
    public boolean getActivationStatus() { return m_activated; }
    public String getName() { return m_name;}
}
