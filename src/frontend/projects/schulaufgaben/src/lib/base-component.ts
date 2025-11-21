import { Component, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";

@Component({
    template: ""
})
export class BaseComponent implements OnDestroy {
    readonly subscriptions = new Subscription();
    public constructor() {
    }
    
    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
}