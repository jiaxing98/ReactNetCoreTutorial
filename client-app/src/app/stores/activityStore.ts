import { makeAutoObservable, runInAction } from "mobx"
import agent from "../api/agent"
import { Activity } from "../models/activity"
import { v4 as uuid } from 'uuid'

export default class ActivityStore {
    activityMap = new Map<string, Activity>()
    selectedActivity: Activity | undefined = undefined
    editMode = false
    loading = false
    loadingInitial = false

    constructor() {
        makeAutoObservable(this)
    }

    get activitiesByDate() {
        return Array.from(this.activityMap.values()).sort((a, b) => 
            Date.parse(a.date) - Date.parse(b.date)
        )
    }

    /** The reason to use runInAction()
     * https://mobx.js.org/actions.html#asynchronous-actions
     */
    loadActivities = async () => {
        this.loadingInitial = true
        try {
            const activities = await agent.Activities.list()
            runInAction(() => {
                activities.forEach(activity => {
                    activity.date = activity.date.split('T')[0]
                    this.activityMap.set(activity.id, activity)
                })
                this.loadingInitial = false
            })
        } catch (error) {
            console.log(error)
            runInAction(() => {
                this.loadingInitial = false
            })
        }
    }

    selectActivity = (id: string) => {
        this.selectedActivity = this.activityMap.get(id)
    }

    cancelSelectedActivity = () => {
        this.selectedActivity = undefined
    }

    openForm = (id?: string) => {
        id ? this.selectActivity(id) : this.cancelSelectedActivity()
        this.editMode = true
    }

    closeForm = () => {
        this.editMode = false
    }

    createActivity = async (activity: Activity) => {
        this.loading = true
        activity.id = uuid()
        try {
            await agent.Activities.create(activity)
            runInAction(() => {
                this.activityMap.set(activity.id, activity)
                this.selectedActivity = activity
                this.editMode = false
                this.loading = false
            })
        } catch (error) {
            console.log(error)
            runInAction(() => {
                this.loading = false
            })
        }
    }

    updateActivity = async (activity: Activity) => {
        this.loading = true
        activity.id = uuid()
        try {
            await agent.Activities.update(activity)
            runInAction(() => {
                this.activityMap.set(activity.id, activity)
                this.selectedActivity = activity
                this.editMode = false
                this.loading = false
            })
        } catch (error) {
            console.log(error)
            runInAction(() => {
                this.loading = false
            })
        }
    }

    deleteActivity = async (id: string) => {
        this.loading = true
        try {
            await agent.Activities.delete(id)
            runInAction(() => {
                this.activityMap.delete(id)
                if (this.selectedActivity?.id === id) this.cancelSelectedActivity()
                this.editMode = false
                this.loading = false
            })
        } catch (error) {
            console.log(error)
            runInAction(() => {
                this.loading = false
            })
        }
    }
}