﻿using System;
using System.Collections.Generic;

namespace SubH
{
public partial class SubHub<TMessage> : ISubHub<TMessage>
	where TMessage : IMessage
{
	private readonly		List<ISubscription<TMessage>>					_subscriptions			= new List<ISubscription<TMessage>>();

	public					ISubscription<TMessage>	Sub						( Action<TMessage> action, int order = 0 )
	{
		var subscription			= new Subscription<TMessage>( false, null, action );
		AddSubscription( subscription );
		return subscription;
	}

	public					ISubscription<TMessage>	Sub						( Object filter, Action<TMessage> action, int order = 0 )
	{
		if ( filter == null )
		{
			throw new ArgumentNullException( "filter == null" );
		}

		var subscription			= new Subscription<TMessage>( true, filter, action );
		AddSubscription( subscription );
		return subscription;
	}

	private					void					AddSubscription			( ISubscription<TMessage> subscription )
	{
		_subscriptions.Add( subscription );
	}

	public					void					Unsub					( ISubscription<TMessage> subscription )
	{
		_subscriptions.Remove( subscription );
	}

	public					void					Publish					( TMessage message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		foreach( var subscription in _subscriptions )
		{
			if ( subscription.HasFilter )
			{
				continue;
			}

			subscription.Invoke( message );
		}
	}

	public					void					Publish					( Object filter, TMessage message )
	{
		if ( message == null )
		{
			throw new ArgumentNullException( "message == null" );
		}

		foreach( var subscription in _subscriptions )
		{
			if ( subscription.HasFilter && subscription.Filter != filter )
			{
				continue;
			}

			subscription.Invoke( message );
		}
	}

	public					void					ClearAllSubscriptions	(  )
	{
		_subscriptions.Clear(  );
	}
}
}