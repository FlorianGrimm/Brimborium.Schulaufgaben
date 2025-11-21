import { BehaviorSubject, Observable, Subscription } from "rxjs";

export function createSubjectObservable<T>(
    args: {
        initialValue: T;
        observable: Observable<T>;
        subscription: Subscription;
    }
): BehaviorSubject<T> {
    const result = new BehaviorSubject<T>(args.initialValue);
    const subscription = new Subscription();
    args.subscription.add(subscription);
    subscription.add(
        args.observable.subscribe({
            next: (value) => {
                result.next(value);
            },
            error: (err) => {
                // result.error(err);
                subscription.unsubscribe();
            },
            complete: () => {
                // result.complete();
                subscription.unsubscribe();
            }
        }));
    return result;
} 